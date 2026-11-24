using System.Diagnostics;
namespace DroidLab.Devices;
public record RestoreResult(string Destination,int FileCount);
public static class SharedStorageRestore {
 public static string Destination(Guid id)=>"/sdcard/Download/DroidLab-Restore-"+id.ToString("N");
 public static string[] PushArguments(Device device,string source,string destination){
 if(device.Transport!="ADB"||device.State!="device"||string.IsNullOrWhiteSpace(device.Serial)||device.Serial.Any(char.IsControl))throw new InvalidOperationException("Authorized online ADB device required.");
 const string prefix="/sdcard/Download/DroidLab-Restore-";
 if(!destination.StartsWith(prefix,StringComparison.Ordinal)||!Guid.TryParseExact(destination[prefix.Length..],"N",out _))throw new ArgumentException("Unique staging destination required.");
 return ["-s",device.Serial,"push",source,destination+"/"];
 }
 static void RejectLinks(string path){
 if((File.GetAttributes(path)&FileAttributes.ReparsePoint)!=0)throw new IOException("Linked backup entries are not supported.");
 if(Directory.Exists(path))foreach(var child in Directory.EnumerateFileSystemEntries(path))RejectLinks(child);
 }
 static async Task Run(string[] args,CancellationToken token){
 using var p=new Process{StartInfo=new ProcessStartInfo("adb"){UseShellExecute=false,RedirectStandardOutput=true,RedirectStandardError=true,CreateNoWindow=true}};
 foreach(var arg in args)p.StartInfo.ArgumentList.Add(arg);
 p.Start();var stdout=p.StandardOutput.ReadToEndAsync();var stderr=p.StandardError.ReadToEndAsync();
 using var timeout=CancellationTokenSource.CreateLinkedTokenSource(token);timeout.CancelAfter(TimeSpan.FromMinutes(30));
 try{await p.WaitForExitAsync(timeout.Token);}catch(OperationCanceledException){if(!p.HasExited)p.Kill(true);throw;}
 await stdout;var error=await stderr;if(p.ExitCode!=0)throw new IOException("ADB operation failed: "+error);
 }
 public static async Task<RestoreResult> RunAsync(Device device,string root,CancellationToken token=default){
 root=Path.GetFullPath(root);var destination=Destination(Guid.NewGuid());var data=Path.Combine(root,"data");
 var args=PushArguments(device,data,destination);
 RejectLinks(root);
 var verified=await BackupVerification.VerifyAsync(root,token);
 if(!verified.Valid)throw new InvalidDataException("Backup verification failed: "+string.Join("; ",verified.Issues));
 var entries=await BackupManifest.ScanAsync(data,token);
 if(entries.Count==0)throw new InvalidDataException("Empty backup.");
 var discovery=await Discovery.ScanAsync("ADB",token);
 if(discovery.Error!=null||!discovery.Devices.Any(d=>d.Serial==device.Serial&&d.State=="device"))throw new InvalidOperationException("Selected device is no longer authorized and online.");
 await Run(["-s",device.Serial,"shell","mkdir",destination],token);
 try{await Run(args,token);}catch(Exception ex){throw new IOException("Restore incomplete; staging directory may remain at "+destination,ex);}
 return new(destination,entries.Count);
 }
}