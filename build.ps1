$ErrorActionPreference = 'Stop'
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$vsPath = $env:VINTAGE_STORY
if ([string]::IsNullOrWhiteSpace($vsPath)) { $vsPath = Join-Path $env:APPDATA 'Vintagestory' }
if (-not (Test-Path (Join-Path $vsPath 'VintagestoryAPI.dll'))) {
    throw "Cannot find VintagestoryAPI.dll in '$vsPath'. Set `$env:VINTAGE_STORY to your installation directory."
}
if (-not (Test-Path (Join-Path $vsPath 'Lib\0Harmony.dll'))) {
    throw "Cannot find Lib\0Harmony.dll in '$vsPath'."
}
& dotnet build (Join-Path $here 'LtoHodWaterCompatV5.csproj') --configuration Release "-p:VintageStoryPath=$vsPath" --nologo
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed" }
$dll = Join-Path $here 'bin\Release\net10.0\LtoHodWaterCompatV5.dll'
if (-not (Test-Path $dll)) { throw "Compiled DLL not found: $dll" }
$staging = Join-Path $here 'package'
if (Test-Path $staging) { Remove-Item $staging -Recurse -Force }
New-Item -Path $staging -ItemType Directory | Out-Null
Copy-Item $dll (Join-Path $staging 'LtoHodWaterCompatV5.dll')
Copy-Item (Join-Path $here 'modinfo.json') (Join-Path $staging 'modinfo.json')
$out = Join-Path $here 'LtoHodWaterCompat_0.2.0-TEST.zip'
if (Test-Path $out) { Remove-Item $out -Force }
Compress-Archive -Path (Join-Path $staging '*') -DestinationPath $out
Write-Host "Built standalone test mod: $out"
Write-Host 'Exit Vintage Story fully. Remove older 0.1.x compatibility ZIPs before installing 0.2.0.'
