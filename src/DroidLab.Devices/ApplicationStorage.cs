using System.Diagnostics;
using System.Text.RegularExpressions;
namespace DroidLab.Devices;
public static class ApplicationStorage {
 public static string[] Packages(string raw,string filter="")=>raw.Split('\n').Select(x=>x.Trim()).Where(x=>x.StartsWith("package:",StringComparison.Ordinal)).Select(x=>x[8..]).Where(x=>ValidPackage(x)&&x.Contains(filter,StringComparison.OrdinalIgnoreCase)).Distinct().OrderBy(x=>x,StringComparer.Ordinal).ToArray();
 static bool ValidPackage(string value)=>Regex.IsMatch(value,@"\A[A-Za-z][A-Za-z0-9_]*(\.[A-Za-z][A-Za-z0-9_]*)+\z");
 public static string[] PackageArguments(string package,bool uninstall=false){
 if(!ValidPackage(package))throw new ArgumentException("Invalid package identifier.");
 return uninstall?["uninstall",package]:["shell","dumpsys","package",package];
 }
 public static string SharedPath(string relative){
 if(relative.Length==0)return "/sdcard";
 if(relative.Split('/').Any(p=>p=="."||p==".."||!Regex.IsMatch(p,@"\A[A-Za-z0-9_][A-Za-z0-9_.-]*\z")))throw new ArgumentException("Use safe relative shared-storage path segments.");
 if(relative.Split('/')[0].Equals("Android",StringComparison.OrdinalIgnoreCase))throw new ArgumentException("Protected app storage is excluded.");
 return "/sdcard/"+relative;
 }
 public static string[] InstallArguments(string path){
 if(!Path.IsPathFullyQualified(path)||!Path.GetExtension(path).Equals(".apk",StringComparison.OrdinalIgnoreCase))throw new ArgumentException("Select a local APK.");
 return ["install",path];
 }
 public static bool IsAllowed(IReadOnlyList<string> args){
 if(args.Any(a=>string.IsNullOrEmpty(a)||a.Any(char.IsControl)))return false;
 if(NetworkDiagnostics.Queries().Any(q=>q.Arguments.SequenceEqual(args)))return true;
 bool Shared(string path){try{return path=="/sdcard"||path.StartsWith("/sdcard/",StringComparison.Ordinal)&&SharedPath(path[8..])==path;}catch(ArgumentException){return false;}}
 if(args.Count==4&&args[0]=="shell"&&args[1]=="dumpsys"&&args[2]=="package")return ValidPackage(args[3]);
 if(args.Count==4&&args[0]=="shell"&&args[1]=="ls"&&args[2]=="-la")return Shared(args[3]);
 if(args.Count==2&&args[0]=="uninstall")return ValidPackage(args[1]);
 if(args.Count==2&&args[0]=="install")return Path.IsPathFullyQualified(args[1])&&Path.GetExtension(args[1]).Equals(".apk",StringComparison.OrdinalIgnoreCase);
 if(args.Count==3&&args[0]=="pull")return args[1]!="/sdcard"&&Shared(args[1])&&Path.IsPathFullyQualified(args[2]);
 return false;
 }
 public static async Task<string> RunAsync(Device device,IReadOnlyList<string> args){
 if(device.Transport!="ADB"||device.State!="device"||string.IsNullOrWhiteSpace(device.Serial)||device.Serial.Any(char.IsControl))throw new InvalidOperationException("Select an authorized online ADB device.");
 if(!IsAllowed(args))throw new ArgumentException("Command outside application/storage/network allowlist.");
 var scan=await Discovery.ScanAsync("ADB");if(scan.Error!=null||!scan.Devices.Any(d=>d.Serial==device.Serial&&d.State=="device"))throw new InvalidOperationException("Device authorization or availability changed.");
 using var p=new Process{StartInfo=new ProcessStartInfo("adb"){UseShellExecute=false,RedirectStandardOutput=true,RedirectStandardError=true,CreateNoWindow=true}};
 foreach(var arg in new[]{"-s",device.Serial}.Concat(args))p.StartInfo.ArgumentList.Add(arg);
 p.Start();var output=p.StandardOutput.ReadToEndAsync();var error=p.StandardError.ReadToEndAsync();using var timeout=new CancellationTokenSource(TimeSpan.FromMinutes(3));
 try{await p.WaitForExitAsync(timeout.Token);}catch(OperationCanceledException){if(!p.HasExited)p.Kill(true);throw new TimeoutException("ADB timed out; verify device state before retrying.");}
 var result=await output;var err=await error;if(p.ExitCode!=0)throw new InvalidOperationException(result+"\n"+err);return result;
 }
}