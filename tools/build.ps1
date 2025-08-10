param(
    [switch]$DetectOnly
)

$ErrorActionPreference = 'Stop'

$root = Resolve-Path "$PSScriptRoot/.."
$configFile = Join-Path $root 'configs/openness.json'
$tiaPath = $null
$publicApiUsed = $null

# Manual override from configs/openness.json
if (Test-Path $configFile) {
    try {
        $json = Get-Content $configFile -Raw
        if ($json.Trim().Length -gt 0) {
            $config = $json | ConvertFrom-Json
            if ($config.tia_path) {
                $candidate = $config.tia_path
                $apiRoot = Join-Path $candidate 'PublicAPI'
                $dll = Get-ChildItem -Path $apiRoot -Filter 'Siemens.Engineering.dll' -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
                if ($dll) {
                    $tiaPath = $candidate
                    $publicApiUsed = $dll.DirectoryName
                }
            }
        }
    } catch {}
}

if (-not $tiaPath) {
    $searchRoots = @('C:\Program Files\Siemens\Automation', 'C:\Program Files (x86)\Siemens\Automation')
    $candidates = @()
    foreach ($rootDir in $searchRoots) {
        if (Test-Path $rootDir) {
            foreach ($portalDir in Get-ChildItem -Path $rootDir -Directory -Filter 'Portal V*' -ErrorAction SilentlyContinue) {
                $versionMatch = [regex]::Match($portalDir.Name, 'Portal V(\d+)')
                if ($versionMatch.Success) {
                    $apiRoot = Join-Path $portalDir.FullName 'PublicAPI'
                    if (Test-Path $apiRoot) {
                        $dll = Get-ChildItem -Path $apiRoot -Filter 'Siemens.Engineering.dll' -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
                        if ($dll) {
                            $candidates += [pscustomobject]@{
                                Version    = [int]$versionMatch.Groups[1].Value
                                TiaPath    = $portalDir.FullName
                                PublicApi  = $dll.DirectoryName
                            }
                        }
                    }
                }
            }
        }
    }

    if ($candidates.Count -gt 0) {
        $selection = $candidates | Sort-Object Version -Descending | Select-Object -First 1
        $tiaPath = $selection.TiaPath
        $publicApiUsed = $selection.PublicApi
    }
}

if ($DetectOnly) {
    if ($tiaPath) {
        Write-Output $tiaPath
        exit 0
    } else {
        Write-Error 'TIA Portal not found'
        exit 1
    }
}

if (-not $tiaPath) {
    Write-Error 'TIA Portal installation not found. Ensure TIA Portal is installed or specify tia_path in configs/openness.json.'
    exit 1
}

# Save detected path
@{ tia_path = $tiaPath } | ConvertTo-Json | Set-Content -Path $configFile

# Prepare destination and copy DLLs
$destLib = Join-Path $root 'src/Agent.OpennessBridge/lib'
New-Item -ItemType Directory -Path $destLib -Force | Out-Null
Get-ChildItem -Path $publicApiUsed -Filter 'Siemens.Engineering*.dll' -Recurse -File | ForEach-Object {
    Copy-Item -Path $_.FullName -Destination $destLib -Force
}

Write-Host "Using TIA Portal path: $tiaPath"
Write-Host "PublicAPI folder: $publicApiUsed"
