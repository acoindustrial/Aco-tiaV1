param([switch]$Offline)

$dist = Join-Path "dist" "AgentTIA_v1_offline"
if(Test-Path $dist){ Remove-Item $dist -Recurse -Force }
New-Item -ItemType Directory -Path $dist | Out-Null

Write-Host "Publishing UI"
dotnet publish ../src/AgentTIA.UI/AgentTIA.UI.csproj -c Release -r win-x64 -p:PublishSingleFile=false --self-contained false -o $dist | Out-Null
Write-Host "Publishing Engine"
dotnet publish ../src/Agent.Engine/Agent.Engine.csproj -c Release -o $dist | Out-Null

Copy-Item ../assets -Destination $dist -Recurse
Copy-Item ../configs -Destination $dist -Recurse
Copy-Item ../README_run.pdf -Destination $dist -ErrorAction SilentlyContinue

Write-Host "Build complete"
