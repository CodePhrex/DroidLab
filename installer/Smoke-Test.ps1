$ErrorActionPreference = 'Stop'
$setup = (Resolve-Path 'dist/DroidLab-Setup-x64.exe').Path
$target = Join-Path $env:RUNNER_TEMP ('DroidLab-smoke-' + [guid]::NewGuid().ToString('N'))
function Invoke-Checked($exe, $arguments) {
  $p = Start-Process -FilePath $exe -ArgumentList $arguments -Wait -PassThru
  if ($p.ExitCode -ne 0) { throw "Process failed: $exe ($($p.ExitCode))" }
}
Invoke-Checked $setup @('/VERYSILENT','/SUPPRESSMSGBOXES','/NORESTART',('/DIR="' + $target + '"'),'/TASKS=""')
$app = Join-Path $target 'DroidLab.Desktop.exe'
$uninstall = Join-Path $target 'unins000.exe'
if (!(Test-Path $app) -or !(Test-Path $uninstall)) { throw 'Installation payload missing' }
try {
  $p = Start-Process -FilePath $app -PassThru
  try {
    Start-Sleep -Seconds 8
    $p.Refresh()
    if ($p.HasExited) { throw "Application exited during startup: $($p.ExitCode)" }
  } finally { if (!$p.HasExited) { Stop-Process -Id $p.Id -Force; $p.WaitForExit() } }
} finally {
  Invoke-Checked $uninstall @('/VERYSILENT','/SUPPRESSMSGBOXES','/NORESTART')
}
if (Test-Path $app) { throw 'Uninstall left executable behind' }
Write-Host 'Install, startup survival and uninstall smoke checks passed.'
