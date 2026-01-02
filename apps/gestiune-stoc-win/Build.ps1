$ErrorActionPreference = 'Stop'

$OutDir = Join-Path $PSScriptRoot 'out'
if (-not (Test-Path $OutDir)) {
    throw "Output directory '$OutDir' not found."
}

$ZipPath = Join-Path $PSScriptRoot 'gestiune-stoc-win.zip'
if (Test-Path $ZipPath) {
    Remove-Item $ZipPath
}

Compress-Archive -Path (Join-Path $OutDir '*') -DestinationPath $ZipPath -Force
