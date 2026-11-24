using System.Text.Json;
namespace DroidLab.Devices;
public record VerificationResult(bool Valid,IReadOnlyList<string> Issues);
public static class BackupVerification {
 public static async Task<VerificationResult> VerifyAsync(string root,CancellationToken token=default){
 var expected=JsonSerializer.Deserialize<List<BackupEntry>>(await File.ReadAllTextAsync(Path.Combine(root,"manifest.json"),token))??throw new InvalidDataException("Missing manifest entries.");
 var actual=await BackupManifest.ScanAsync(Path.Combine(root,"data"),token);
 var issues=new List<string>();var comparer=StringComparer.Ordinal;
 var map=actual.ToDictionary(e=>e.RelativePath,comparer);var seen=new HashSet<string>(comparer);
 foreach(var e in expected){if(!seen.Add(e.RelativePath)){issues.Add("Duplicate: "+e.RelativePath);continue;}if(!map.TryGetValue(e.RelativePath,out var a))issues.Add("Missing: "+e.RelativePath);else if(e.Bytes!=a.Bytes||!string.Equals(e.Sha256,a.Sha256,StringComparison.OrdinalIgnoreCase))issues.Add("Changed: "+e.RelativePath);}
 foreach(var a in actual)if(!seen.Contains(a.RelativePath))issues.Add("Unexpected: "+a.RelativePath);
 return new(issues.Count==0,issues);
 }
}