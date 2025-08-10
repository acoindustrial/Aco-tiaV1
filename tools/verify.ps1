$dist = Join-Path "dist" "AgentTIA_v1_offline"
if(!(Test-Path $dist)){ Write-Error "Distribution not found"; exit 1 }
Get-ChildItem -Path $dist -Recurse -File | Get-FileHash | ForEach-Object {"$($_.Hash) `t$($_.Path)"} | Set-Content (Join-Path $dist "hashes.txt")
Write-Host "Hashes written to $dist/hashes.txt"
