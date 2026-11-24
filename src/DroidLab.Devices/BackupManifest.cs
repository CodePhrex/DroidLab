using System.Security.Cryptography;
using System.Text.Json;
namespace DroidLab.Devices;
public record BackupEntry(string RelativePath,long Bytes,string Sha256);
public static class BackupManifest {
 public static async Task<IReadOnlyList<BackupEntry>> ScanAsync(string directory,CancellationToken token=default){
 var root=Path.GetFullPath(directory);if((File.GetAttributes(root)&FileAttributes.ReparsePoint)!=0)throw new IOException("Linked root not supported.");
 var entries=new List<BackupEntry>();
 var options=new EnumerationOptions{RecurseSubdirectories=true,AttributesToSkip=FileAttributes.ReparsePoint,IgnoreInaccessible=false};
 foreach(var path in Directory.EnumerateFiles(root,"*",options).OrderBy(p=>p,StringComparer.Ordinal)){
 token.ThrowIfCancellationRequested();await using var file=File.OpenRead(path);
 entries.Add(new(Path.GetRelativePath(root,path),file.Length,Convert.ToHexString(await SHA256.HashDataAsync(file,token))));
 }return entries;
 }
 public static string Serialize(IReadOnlyList<BackupEntry> entries)=>JsonSerializer.Serialize(entries,new JsonSerializerOptions{WriteIndented=true});
}