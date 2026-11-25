using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace DroidLab.Devices;
public record BulletinEntry(string Cve,string PatchLevel,string[] AndroidVersions,string Source);
public static class BulletinCorrelation {
 static bool Date(string value,out DateOnly date)=>DateOnly.TryParseExact(value,"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None,out date);
 public static IReadOnlyList<BulletinEntry> Parse(string json){
 var entries=JsonSerializer.Deserialize<List<BulletinEntry>>(json)??throw new InvalidDataException("Missing bulletin records.");
 foreach(var e in entries){
 if(e==null||e.Cve==null||!Regex.IsMatch(e.Cve,@"^CVE-[0-9]{4}-[0-9]{4,}$")||!Date(e.PatchLevel,out var patch)||(patch.Day!=1&&patch.Day!=5)||e.AndroidVersions==null||e.AndroidVersions.Length==0||e.AndroidVersions.Any(v=>string.IsNullOrWhiteSpace(v))||!Uri.TryCreate(e.Source,UriKind.Absolute,out var uri)||uri.Scheme!="https"||uri.Host!="source.android.com"||!uri.AbsolutePath.StartsWith("/docs/security/bulletin/",StringComparison.Ordinal))throw new InvalidDataException("Invalid bulletin record.");
 }
 return entries;
 }
 public static IReadOnlyList<SecurityFinding> Assess(DeviceIdentity identity,IReadOnlyList<BulletinEntry> entries,DateOnly today){
 var valid=Date(identity.SecurityPatch,out var patch)&&patch<=today;
 return entries.Select(e=>{
 Date(e.PatchLevel,out var required);
 var known=e.AndroidVersions.Contains(identity.AndroidVersion,StringComparer.Ordinal);
 var status=!valid||required>today||!known?"Unknown":patch>=required?"Reported patch coverage":"Potential exposure";
 return new SecurityFinding(e.Cve,status,$"Required patch: {e.PatchLevel}; reported: {identity.SecurityPatch}; Android: {identity.AndroidVersion}; listed versions: {string.Join(", ",e.AndroidVersions)}; source: {e.Source}. Not a confirmed vulnerability or fix.");
 }).ToList();
 }
}