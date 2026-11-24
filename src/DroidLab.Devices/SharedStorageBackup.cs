using System.Diagnostics;
namespace DroidLab.Devices;
public enum BackupFolder { DCIM, Pictures, Documents, Download, Movies, Music }
public record BackupResult(string Directory,int FileCount,string ManifestPath);
public static class SharedStorageBackup {
 public static string[] Arguments(Device device,BackupFolder folder,string destination){
 if(device.Transport!="ADB"||device.State!="device"||string.IsNullOrWhiteSpace(device.Serial)||device.Serial.Any(char.IsControl))throw new InvalidOperationException("Authorized online ADB device required.");
 if(!Enum.IsDefined(folder))throw new ArgumentOutOfRangeException(nameof(folder));
 return ["-s",device.Serial,"pull","/sdcard/"+folder,destination];
 }
 public static async Task<BackupResult> RunAsync(Device device,BackupFolder folder,string parent,CancellationToken token=default){
 var root=Path.Combine(Path.GetFullPath(parent),"DroidLab-"+Guid.NewGuid().ToString("N"));
 var data=Path.Combine(root,"data");var args=Arguments(device,folder,data);
 Directory.CreateDirectory(data);
 using var p=new Process{StartInfo=new ProcessStartInfo("adb"){UseShellExecute=false,RedirectStandardOutput=true,RedirectStandardError=true,CreateNoWindow=true}};
 foreach(var arg in args)p.StartInfo.ArgumentList.Add(arg);
 p.Start();var output=p.StandardOutput.ReadToEndAsync();var error=p.StandardError.ReadToEndAsync();
 using var timeout=CancellationTokenSource.CreateLinkedTokenSource(token);timeout.CancelAfter(TimeSpan.FromMinutes(30));
 try{await p.WaitForExitAsync(timeout.Token);}catch(OperationCanceledException){if(!p.HasExited)p.Kill(true);throw;}
 await output;if(p.ExitCode!=0)throw new IOException("Backup incomplete: "+await error);
 var entries=await BackupManifest.ScanAsync(data,token);
 var manifest=Path.Combine(root,"manifest.json");await File.WriteAllTextAsync(manifest,BackupManifest.Serialize(entries),token);
 return new(root,entries.Count,manifest);
 }
}