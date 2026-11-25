using System.Diagnostics;
namespace DroidLab.Devices;
public record WorkbenchCommand(string Id,string Transport,string Description);
public static class CommandWorkbench {
 public static IReadOnlyList<WorkbenchCommand> Catalog(string transport,string filter="")=>new WorkbenchCommand[]{
 new("battery","ADB","Battery status"),new("memory","ADB","Memory information"),new("storage","ADB","Filesystem usage"),new("logcat","ADB","Recent Logcat warnings"),new("uptime","ADB","System uptime"),new("product","Fastboot","Product identifier"),new("bootloader","Fastboot","Bootloader version"),new("slot","Fastboot","Current slot"),new("secure","Fastboot","Reported secure state")
 }.Where(c=>c.Transport==transport&&(c.Id.Contains(filter,StringComparison.OrdinalIgnoreCase)||c.Description.Contains(filter,StringComparison.OrdinalIgnoreCase))).ToArray();
 public static string[] Arguments(Device device,string id){
 if(string.IsNullOrWhiteSpace(device.Serial)||device.Serial.Any(char.IsControl))throw new ArgumentException("Invalid serial.");
 if(!Catalog(device.Transport).Any(c=>c.Id==id))throw new ArgumentException("Unsupported command for transport.");
 if(device.Transport=="ADB"&&device.State!="device"||device.Transport=="Fastboot"&&device.State!="fastboot")throw new InvalidOperationException("Device is not ready.");
 string[] args=id switch{
 "battery"=>["shell","dumpsys","battery"],"memory"=>["shell","cat","/proc/meminfo"],"storage"=>["shell","df","-h"],"logcat"=>["logcat","-d","-t","200","*:W"],"uptime"=>["shell","uptime"],"product"=>["getvar","product"],"bootloader"=>["getvar","version-bootloader"],"slot"=>["getvar","current-slot"],"secure"=>["getvar","secure"],_=>throw new ArgumentException("Unsupported command.")};
 return ["-s",device.Serial,..args];
 }
 public static async Task<string> RunAsync(Device device,string id){
 var args=Arguments(device,id);var scan=await Discovery.ScanAsync(device.Transport);
 if(scan.Error!=null||!scan.Devices.Any(d=>d.Serial==device.Serial&&d.State==device.State))throw new InvalidOperationException("Device no longer ready; rescan.");
 using var p=new Process{StartInfo=new ProcessStartInfo(device.Transport=="ADB"?"adb":"fastboot"){UseShellExecute=false,RedirectStandardOutput=true,RedirectStandardError=true,CreateNoWindow=true}};
 foreach(var arg in args)p.StartInfo.ArgumentList.Add(arg);
 p.Start();var output=p.StandardOutput.ReadToEndAsync();var error=p.StandardError.ReadToEndAsync();using var timeout=new CancellationTokenSource(TimeSpan.FromSeconds(30));
 try{await p.WaitForExitAsync(timeout.Token);}catch(OperationCanceledException){if(!p.HasExited)p.Kill(true);throw new TimeoutException("Command timed out.");}
 var result=await output+"\n"+await error;if(p.ExitCode!=0)throw new InvalidOperationException(result);return result;
 }
}