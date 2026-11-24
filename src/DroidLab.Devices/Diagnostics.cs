using System.Diagnostics;
namespace DroidLab.Devices;
public enum DiagnosticKind { Identity, Battery, Storage, Memory, Applications }
public static class Diagnostics {
 public static IReadOnlyList<string> Arguments(string serial,DiagnosticKind kind){
 if(string.IsNullOrWhiteSpace(serial)||serial.Any(char.IsControl))throw new ArgumentException("Invalid serial");
 string[] command=kind switch {
 DiagnosticKind.Identity=>["shell","getprop"],
 DiagnosticKind.Battery=>["shell","dumpsys","battery"],
 DiagnosticKind.Storage=>["shell","df","-h"],
 DiagnosticKind.Memory=>["shell","cat","/proc/meminfo"],
 DiagnosticKind.Applications=>["shell","pm","list","packages"],
 _=>throw new ArgumentOutOfRangeException(nameof(kind))};
 return ["-s",serial,..command];
 }
 public static async Task<string> ReadAsync(Device device,DiagnosticKind kind){
 if(device.Transport!="ADB"||device.State!="device")throw new InvalidOperationException("Select an authorized online ADB device.");
 using var p=new Process{StartInfo=new ProcessStartInfo("adb"){UseShellExecute=false,RedirectStandardOutput=true,RedirectStandardError=true,CreateNoWindow=true}};
 foreach(var arg in Arguments(device.Serial,kind))p.StartInfo.ArgumentList.Add(arg);
 p.Start();var output=p.StandardOutput.ReadToEndAsync();var error=p.StandardError.ReadToEndAsync();
 using var timeout=new CancellationTokenSource(TimeSpan.FromSeconds(25));
 try{await p.WaitForExitAsync(timeout.Token);}catch(OperationCanceledException){if(!p.HasExited)p.Kill(true);throw new TimeoutException("Diagnostic timed out.");}
 if(p.ExitCode!=0)throw new InvalidOperationException(await error);
 return await output;
 }
}