using System.Diagnostics;
namespace DroidLab.Devices;
public record Device(string Serial,string State,string Transport,string Details);
public record DiscoveryResult(string Transport,IReadOnlyList<Device> Devices,string? Error);
public static class Discovery {
 public static IReadOnlyList<Device> Parse(string output,string transport){
 if(transport!="ADB" && transport!="Fastboot") throw new ArgumentException("Unknown transport");
 var devices=new List<Device>();
 foreach(var line in output.Split('\n')){
 var s=line.Trim(); if(s.Length==0||s.StartsWith("List of devices")||s.StartsWith("*")||s.StartsWith("error:"))continue;
 var parts=s.Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries);
 if(parts.Length<2)continue;
 if(transport=="Fastboot"&&parts[1]!="fastboot")continue;
 if(transport=="ADB"&&!new[]{"device","offline","unauthorized","recovery","sideload","bootloader","no"}.Contains(parts[1]))continue;
 devices.Add(new(parts[0],parts[1]=="no"?"no permissions":parts[1],transport,string.Join(" ",parts.Skip(2))));
 }return devices;
 }
 public static async Task<DiscoveryResult> ScanAsync(string transport,CancellationToken cancellationToken=default){
 if(transport!="ADB"&&transport!="Fastboot")throw new ArgumentException("Unknown transport");
 var exe=transport=="ADB"?"adb":"fastboot";
 try{
 using var p=new Process{StartInfo=new ProcessStartInfo(exe){UseShellExecute=false,RedirectStandardOutput=true,RedirectStandardError=true,CreateNoWindow=true}};
 p.StartInfo.ArgumentList.Add("devices");if(transport=="ADB")p.StartInfo.ArgumentList.Add("-l");
 p.Start();var stdout=p.StandardOutput.ReadToEndAsync();var stderr=p.StandardError.ReadToEndAsync();
 using var timeout=CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);timeout.CancelAfter(TimeSpan.FromSeconds(20));
 try{await p.WaitForExitAsync(timeout.Token);}catch(OperationCanceledException){if(!p.HasExited)p.Kill(true);throw;}
 var output=await stdout;var error=await stderr;
 return p.ExitCode==0?new(transport,Parse(output,transport),null):new(transport,[],error);
 }catch(Exception ex) when(ex is not OperationCanceledException || !cancellationToken.IsCancellationRequested){return new(transport,[],ex.Message);}
 }
}