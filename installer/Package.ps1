$ErrorActionPreference = 'Stop'
$compiler = Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6\ISCC.exe'
if (!(Test-Path $compiler)) { throw 'Inno Setup 6 compiler not installed' }
if (!(Test-Path 'publish/DroidLab.Desktop.exe')) { throw 'Publish payload missing' }
& $compiler 'installer/DroidLab.iss'
if ($LASTEXITCODE -ne 0) { throw 'Installer compilation failed' }
Compress-Archive -Path 'publish/*' -DestinationPath 'dist/DroidLab-Portable-x64.zip' -Force
$lines = Get-ChildItem 'dist' -File | Where-Object { $_.Extension -in '.exe','.zip' } | Sort-Object Name | ForEach-Object { (Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant() + '  ' + $_.Name }
if (@($lines).Count -ne 2) { throw 'Expected two release artifacts' }
$lines | Set-Content 'dist/SHA256SUMS.txt' -Encoding ascii
