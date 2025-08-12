param(
    [switch]$DetectOnly,
    [Alias('SkipOpenness')][switch]$Offline
)

$ErrorActionPreference = 'Stop'

$root = Resolve-Path "$PSScriptRoot/.."
$configFile = Join-Path $root 'configs/openness.json'
$tiaPath = $null
$publicApiUsed = $null
$offlineMode = $Offline.IsPresent

if ($DetectOnly -and $offlineMode) {
    Write-Warning 'DetectOnly ignored in Offline mode.'
    exit 0
}

if (-not $offlineMode) {
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
        $searchRoots = @('C:\\Program Files\\Siemens\\Automation', 'C:\\Program Files (x86)\\Siemens\\Automation')
        $candidates = @()
        foreach ($rootDir in $searchRoots) {
            if (Test-Path $rootDir) {
                foreach ($portalDir in Get-ChildItem -Path $rootDir -Directory -Filter 'Portal V*' -ErrorAction SilentlyContinue) {
                    $versionMatch = [regex]::Match($portalDir.Name, 'Portal V(\\d+)')
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
        Write-Warning 'TIA Portal installation not found. Continuing in offline mode.'
        $offlineMode = $true
    } else {
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

if ($offlineMode) {
    $dist = Join-Path $root 'dist/AgentTIA_v1_offline'
    Remove-Item -Recurse -Force $dist -ErrorAction SilentlyContinue
    New-Item -ItemType Directory -Path $dist -Force | Out-Null

    # Build/publish .NET 8 projects
    $projects = Get-ChildItem -Path $root -Filter '*.csproj' -Recurse -ErrorAction SilentlyContinue
    foreach ($proj in $projects) {
        $projName = [System.IO.Path]::GetFileNameWithoutExtension($proj.Name)
        $outDir = Join-Path $dist $projName
        dotnet publish $proj.FullName -c Release -f net8.0 -o $outDir | Out-Null
    }

    # Copy assets and configs
    foreach ($folder in 'assets','configs') {
        $src = Join-Path $root $folder
        if (Test-Path $src -PathType Container) {
            Copy-Item -Path $src -Destination $dist -Recurse -Force
        } elseif (Test-Path $src -PathType Leaf) {
            Copy-Item -Path $src -Destination $dist -Force
        }
    }

    # Include analyzer if exists
    $analyzePy = Join-Path $root 'tools/analyze.py'
    $ruleEngine = Join-Path $root 'src/rule_engine'
    if (Test-Path $analyzePy -or Test-Path $ruleEngine) {
        $analyzerDest = Join-Path $dist 'analyzer'
        New-Item -ItemType Directory -Path $analyzerDest -Force | Out-Null
        if (Test-Path $analyzePy) {
            Copy-Item -Path $analyzePy -Destination $analyzerDest -Force
        }
        if (Test-Path $ruleEngine) {
            Copy-Item -Path $ruleEngine -Destination $analyzerDest -Recurse -Force
        }
    }

    # Write README_run.txt
    $readmeContent = @"
This package was built in offline mode.
Run the published binaries from the folders above to execute.
"@
    Set-Content -Path (Join-Path $dist 'README_run.txt') -Value $readmeContent
}

