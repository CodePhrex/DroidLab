$ErrorActionPreference = 'Stop'
[xml]$xaml = Get-Content src/DroidLab.Desktop/MainWindow.xaml -Raw
$ns = [System.Xml.XmlNamespaceManager]::new($xaml.NameTable)
$ns.AddNamespace('w','http://schemas.microsoft.com/winfx/2006/xaml/presentation')
$ns.AddNamespace('x','http://schemas.microsoft.com/winfx/2006/xaml')
$ns.AddNamespace('c','clr-namespace:DroidLab.Desktop.Controls')
foreach ($name in @('BatteryMeter','MemoryMeter','StorageMeter')) {
  $meter = $xaml.SelectSingleNode("//c:RadialMeter[@x:Name='$name']", $ns)
  if (!$meter -or $meter.GetAttribute('IsValueKnown') -ne 'False' -or !$meter.GetAttribute('AutomationProperties.Name')) { throw "Invalid initial telemetry state: $name" }
}
$knob = $xaml.SelectSingleNode("//c:RotaryKnob[@x:Name='DiscoveryIntervalKnob']", $ns)
if (!$knob) { throw 'Discovery knob missing' }
foreach ($pair in @{Minimum='10';Maximum='60';Value='10';SmallChange='5';TickFrequency='5';IsSnapToTickEnabled='True';ValueChanged='DiscoveryIntervalChanged'}.GetEnumerator()) {
  if ($knob.GetAttribute($pair.Key) -ne $pair.Value) { throw "Invalid knob setting: $($pair.Key)" }
}
foreach ($tag in @('TextBox','ComboBox')) {
  $style = if ($tag -eq 'TextBox') { '{StaticResource DroidLabTextInput}' } else { '{StaticResource DroidLabDropdown}' }
  foreach ($node in $xaml.SelectNodes("//w:$tag", $ns)) {
    if ($node.GetAttribute('Style') -ne $style) { throw "Unstyled workspace $tag" }
  }
}
Write-Host 'UI-03 static XAML regression checks passed.'
