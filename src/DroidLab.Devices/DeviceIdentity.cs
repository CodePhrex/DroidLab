using System.Text.RegularExpressions;
namespace DroidLab.Devices;
public record DeviceIdentity(string Manufacturer,string Model,string Product,string AndroidVersion,string SecurityPatch,string BuildFingerprint,string BootState);
public static class IdentityParser {
 public static DeviceIdentity Parse(string output){
 var props=new Dictionary<string,string>(StringComparer.Ordinal);
 foreach(var line in output.Split('\n')){var m=Regex.Match(line.Trim(),@"^\[([^\]]+)\]: \[(.*)\]$");if(m.Success)props[m.Groups[1].Value]=m.Groups[2].Value;}
 string Get(string key)=>props.GetValueOrDefault(key,"");
 return new(Get("ro.product.manufacturer"),Get("ro.product.model"),Get("ro.product.device"),Get("ro.build.version.release"),Get("ro.build.version.security_patch"),Get("ro.build.fingerprint"),Get("ro.boot.verifiedbootstate"));
 }
}