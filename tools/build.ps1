param(
    [switch]$DetectOnly,
    [Alias('SkipOpenness')][switch]$Offline
)

$ErrorActionPreference = 'Stop'

$root = Resolve-Path "$PSScriptRoot/.."
$configFile = Join-Path $root 'configs/openness.json'
$tiaPath = $null
$publicApiUsed = $null

function BuildOffline {
    param()

    $distRoot = Join-Path $root 'dist/AgentTIA_v1_offline'
    if (Test-Path $distRoot) { Remove-Item $distRoot -Recurse -Force }
    New-Item -ItemType Directory -Path $distRoot -Force | Out-Null

    # Publish .NET UI
    $uiProject = Get-ChildItem -Path (Join-Path $root 'src') -Filter 'AgentTIA.UI.csproj' -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($uiProject) {
        if (Get-Command dotnet -ErrorAction SilentlyContinue) {
            dotnet publish $uiProject.FullName -c Release -o $distRoot | Out-Null
        } else {
            Write-Warning 'dotnet not found, skipping AgentTIA.UI publish.'
        }
    } else {
        Write-Warning 'AgentTIA.UI project not found, skipping publish.'
    }

    # Publish other .NET projects (engines)
    if (Get-Command dotnet -ErrorAction SilentlyContinue) {
        $projects = Get-ChildItem -Path (Join-Path $root 'src') -Filter '*.csproj' -Recurse -ErrorAction SilentlyContinue | Where-Object { $_.Name -ne 'AgentTIA.UI.csproj' }
        foreach ($proj in $projects) {
            $outDir = Join-Path $distRoot $proj.BaseName
            dotnet publish $proj.FullName -c Release -o $outDir | Out-Null
        }
    }

    # Package Python analyzer
    $analyzer = Join-Path $root 'tools/analyze.py'
    if (Test-Path $analyzer) {
        $analyzerDest = Join-Path $distRoot 'analyzer'
        New-Item -ItemType Directory -Path $analyzerDest -Force | Out-Null
        Copy-Item $analyzer $analyzerDest -Force
        $ruleSrc = Join-Path $root 'src/rule_engine'
        if (Test-Path $ruleSrc) {
            Copy-Item $ruleSrc $analyzerDest -Recurse -Force
        }
    }

    # Copy assets and configs
    foreach ($name in @('assets', 'configs')) {
        $src = Join-Path $root $name
        if (Test-Path $src) {
            Copy-Item $src -Destination $distRoot -Recurse -Force
        }
    }

    # README for offline run
    $readme = @"
Offline run steps:

1. Ensure .NET 8 runtime is installed.
2. Run the UI: dotnet AgentTIA.UI.dll (from this folder).
3. Optionally run analyzer: python analyzer/analyze.py
"@
    Set-Content -Path (Join-Path $distRoot 'README_run.txt') -Value $readme

    Write-Host "Offline package created at $distRoot"
}

# Only search for TIA when not in offline mode
if (-not $Offline) {
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
        Write-Warning 'TIA Portal installation not found. Building in offline mode.'
        $Offline = $true
    } else {
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
    }
}

if ($Offline) {
    BuildOffline
    exit 0
}
