param([string]$Directory = 'dist')
$ErrorActionPreference = 'Stop'
$expected = @('DroidLab-Setup-x64.exe','DroidLab-Portable-x64.zip')
$seen = @{}
foreach ($line in Get-Content (Join-Path $Directory 'SHA256SUMS.txt')) {
  if ($line -cnotmatch '^([0-9a-f]{64})  (DroidLab-Setup-x64\.exe|DroidLab-Portable-x64\.zip)$') { throw 'Invalid checksum entry' }
  $hash = $Matches[1]; $name = $Matches[2]
  if ($seen.ContainsKey($name)) { throw 'Duplicate checksum entry' }
  $seen[$name] = $true
  $actual = (Get-FileHash (Join-Path $Directory $name) -Algorithm SHA256).Hash
  if ($actual -ine $hash) { throw "Checksum mismatch: $name" }
}
foreach ($name in $expected) { if (!$seen.ContainsKey($name)) { throw "Missing checksum: $name" } }
Add-Type -AssemblyName System.IO.Compression.FileSystem
$zip = [IO.Compression.ZipFile]::OpenRead((Resolve-Path (Join-Path $Directory 'DroidLab-Portable-x64.zip')).Path)
try {
  foreach ($name in @('DroidLab.Desktop.exe','DroidLab.Desktop.dll','DroidLab.Devices.dll','coreclr.dll')) {
    if (!$zip.GetEntry($name)) { throw "Portable payload missing: $name" }
  }
} finally { $zip.Dispose() }
Write-Host 'Release artifact hashes and portable payload verified.'
