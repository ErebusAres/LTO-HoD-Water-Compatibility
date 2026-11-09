# Quick structural check; not a runtime gameplay test.
$ErrorActionPreference = 'Stop'
$zipPath = Join-Path $PSScriptRoot 'LtoHodWaterCompat_0.2.0-TEST.zip'
if (-not (Test-Path $zipPath)) { $zipPath = Join-Path $PSScriptRoot 'dist\LtoHodWaterCompat_0.2.0-TEST.zip' }
if (-not (Test-Path $zipPath)) { throw 'Compiled compatibility ZIP not found.' }
Add-Type -AssemblyName System.IO.Compression
$z = [System.IO.Compression.ZipFile]::OpenRead($zipPath)
try {
    $expected = @('LtoHodWaterCompatV5.dll', 'modinfo.json')
    $actual = @($z.Entries | ForEach-Object { $_.FullName })
    foreach ($entry in $expected) { if ($actual -notcontains $entry) { throw "Missing ZIP entry: $entry" } }
    if ($actual.Count -ne 2) { throw "Expected exactly two entries; found $($actual.Count)." }
    Write-Host "Package structure OK: $zipPath"
    Get-FileHash -Algorithm SHA256 $zipPath | Format-List
}
finally { $z.Dispose() }
