using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
namespace DroidLab.Devices;
public record AdvisorySnapshot(string Keyword,DateTimeOffset RetrievedAt,IReadOnlyList<Advisory> Advisories);
public sealed class AdvisoryCache(string directory){
 private string PathFor(string keyword)=>Path.Combine(directory,Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(keyword.Trim().ToUpperInvariant())))+".json");
 public async Task SaveAsync(AdvisorySnapshot snapshot,CancellationToken token=default){
 Directory.CreateDirectory(directory);var path=PathFor(snapshot.Keyword);var temp=path+"."+Guid.NewGuid().ToString("N")+".tmp";
 try{await File.WriteAllTextAsync(temp,JsonSerializer.Serialize(snapshot),token);File.Move(temp,path,true);}finally{if(File.Exists(temp))File.Delete(temp);}
 }
 public async Task<AdvisorySnapshot?> ReadAsync(string keyword,CancellationToken token=default){
 var path=PathFor(keyword);if(!File.Exists(path))return null;
 var snapshot=JsonSerializer.Deserialize<AdvisorySnapshot>(await File.ReadAllTextAsync(path,token));
 if(snapshot==null||!string.Equals(snapshot.Keyword.Trim(),keyword.Trim(),StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Cache keyword mismatch.");
 return snapshot;
 }
 public static string Report(AdvisorySnapshot snapshot)=>JsonSerializer.Serialize(new {SchemaVersion=1,Source="NVD and CISA KEV",Scope="Public keyword matches only; not confirmed device vulnerabilities; maximum 50 results.",Snapshot=snapshot},new JsonSerializerOptions{WriteIndented=true});
}