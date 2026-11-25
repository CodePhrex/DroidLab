using System.Globalization;
using System.Text.RegularExpressions;
namespace DroidLab.Devices;
public record DiagnosticMeters(double? Battery,double? MemoryUsed,double? StorageUsed){
 static long? Read(string raw,string key){var m=Regex.Match(raw,@"(?m)^\s*"+Regex.Escape(key)+@":\s*(\d+)\b");return m.Success&&long.TryParse(m.Groups[1].Value,NumberStyles.None,CultureInfo.InvariantCulture,out var n)?n:null;}
 static double? Ratio(long? n,long? d)=>n is >=0&&d is >0&&n<=d?100.0*n.Value/d.Value:null;
 public static DiagnosticMeters Parse(string battery,string memory,string storage){
 var charge=Ratio(Read(battery,"level"),Read(battery,"scale"));
 var free=Ratio(Read(memory,"MemAvailable"),Read(memory,"MemTotal"));
 var row=storage.Split('\n').Select(s=>s.Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries)).FirstOrDefault(p=>p.Length>=6&&p[^1]=="/data");
 double? used=row!=null&&row[^2].EndsWith('%')&&int.TryParse(row[^2].TrimEnd('%'),out var n)&&n>=0&&n<=100?n:null;
 return new(charge,free.HasValue?100-free.Value:null,used);
 }
 public static bool CanRead(Device? device)=>device is {Transport:"ADB",State:"device"};
}