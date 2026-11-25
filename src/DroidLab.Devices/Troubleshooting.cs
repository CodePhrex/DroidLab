using System.Globalization;
using System.Text.RegularExpressions;
namespace DroidLab.Devices;
public record TroubleshootingFinding(string Category,string Severity,string Evidence,string Recommendation);
public static class Troubleshooting {
 static long? Read(string raw,string key){
 var m=Regex.Match(raw,@"(?m)^\s*"+Regex.Escape(key)+@":\s*(\d+)\b");
 return m.Success&&long.TryParse(m.Groups[1].Value,NumberStyles.None,CultureInfo.InvariantCulture,out var n)?n:null;
 }
 public static IReadOnlyList<TroubleshootingFinding> Assess(string battery,string storage,string memory){
 var results=new List<TroubleshootingFinding>();
 var level=Read(battery,"level");var scale=Read(battery,"scale");
 double? percent=level is >=0&&scale is >0&&level<=scale?100.0*level.Value/scale.Value:null;
 results.Add(new("Battery charge",percent is null?"Unknown":percent<=10?"High":percent<=20?"Review":"Observed",percent is null?"Charge unavailable":$"{percent:F0}%","Charge the device if low; charge level does not measure battery capacity."));
 var temp=Read(battery,"temperature");
 results.Add(new("Battery temperature",temp is null||temp>1000?"Unknown":temp>=500?"High":temp>=450?"Review":"Observed",temp is null||temp>1000?"Temperature unavailable":$"{temp/10.0:F1} C","If unusually hot, stop demanding tasks and charging, and allow cooling; seek service if persistent."));
 var health=Read(battery,"health");
 results.Add(new("Battery health",health is null or 0 or 1?"Unknown":health==2?"Observed":"Review",health is null?"Unavailable":$"Android health code {health}","Review reported health; this is not a measured capacity or cycle count."));
 var row=storage.Split('\n').Select(s=>s.Trim().Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries)).FirstOrDefault(p=>p.Length>=6&&p[^1]=="/data");
 int usage=0;bool hasUsage=row!=null&&row[^2].EndsWith('%')&&int.TryParse(row[^2].TrimEnd('%'),out usage)&&usage>=0&&usage<=100;
 results.Add(new("Data storage",!hasUsage?"Unknown":usage>=95?"High":usage>=85?"Review":"Observed",hasUsage?$"/data {usage}% used":"Data filesystem usage unavailable","Review large files and back up important data before manually freeing space."));
 var total=Read(memory,"MemTotal");var available=Read(memory,"MemAvailable");
 double? free=total is >0&&available is >=0&&available<=total?100.0*available.Value/total.Value:null;
 results.Add(new("Available memory",free is null?"Unknown":free<=5?"High":free<=10?"Review":"Observed",free is null?"Memory availability unknown":$"{free:F1}% available","Review demanding apps and repeat measurement; a single snapshot does not establish persistent pressure."));
 return results;
 }
}