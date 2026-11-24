using System.Security.Cryptography;
using System.IO.Compression;
namespace DroidLab.Devices;
public record FirmwareInspection(string Name,long Bytes,string Sha256,bool? MatchesExpected,IReadOnlyList<string> Entries);
public static class FirmwareInspector {
 public static async Task<FirmwareInspection> InspectAsync(string path,string? expected=null,CancellationToken token=default){
 if(expected is not null&&(expected.Length!=64||!expected.All(Uri.IsHexDigit)))throw new ArgumentException("Expected SHA-256 must contain 64 hexadecimal characters.");
 await using var stream=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read,65536,true);
 var hash=Convert.ToHexString(await SHA256.HashDataAsync(stream,token));
 var entries=new List<string>();
 if(Path.GetExtension(path).Equals(".zip",StringComparison.OrdinalIgnoreCase)){
 stream.Position=0;using var archive=new ZipArchive(stream,ZipArchiveMode.Read,true);
 foreach(var entry in archive.Entries.Take(1000)){token.ThrowIfCancellationRequested();entries.Add(entry.FullName);}
 }
 return new(Path.GetFileName(path),stream.Length,hash,expected is null?null:hash.Equals(expected,StringComparison.OrdinalIgnoreCase),entries);
 }
}