using System.Diagnostics;
namespace DroidLab.Devices;
public enum RebootTarget { System, Recovery, Bootloader }
public static class DeviceReboot {
 public static string[] Arguments(Device device,RebootTarget target){
 if(device.Transport!="ADB"||device.State!="device"||string.IsNullOrWhiteSpace(device.Serial)||device.Serial.Any(char.IsControl))throw new InvalidOperationException("Authorized online ADB required.");
 return target switch {RebootTarget.System=>["-s",device.Serial,"reboot"],RebootTarget.Recovery=>["-s",device.Serial,"reboot","recovery"],RebootTarget.Bootloader=>["-s",device.Serial,"reboot","bootloader"],_=>throw new ArgumentOutOfRangeException(nameof(target))};
 }
 public static async Task RequestAsync(Device device,RebootTarget target,CancellationToken token=default){
 var info=new ProcessStartInfo("adb"){UseShellExecute=false,RedirectStandardError=true,CreateNoWindow=true};foreach(var a in Arguments(device,target))info.ArgumentList.Add(a);
 using var p=Process.Start(info)??throw new IOException("Unable to start ADB.");var error=p.StandardError.ReadToEndAsync();
 using var timeout=CancellationTokenSource.CreateLinkedTokenSource(token);timeout.CancelAfter(TimeSpan.FromSeconds(20));
 try{await p.WaitForExitAsync(timeout.Token);}catch(OperationCanceledException){if(!p.HasExited)p.Kill(true);throw;}
 if(p.ExitCode!=0)throw new IOException(await error);
 }
}