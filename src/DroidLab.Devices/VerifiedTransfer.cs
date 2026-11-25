using System.Security.Cryptography;
namespace DroidLab.Devices;
public sealed record TransferReceipt(long Bytes,string Sha256,string Destination);
public static class VerifiedTransfer{
 public static async Task<TransferReceipt> CopyAsync(Stream source,string destination,long expectedBytes,IProgress<long>? progress=null,CancellationToken cancellationToken=default){
 if(expectedBytes<0)throw new ArgumentOutOfRangeException(nameof(expectedBytes));
 destination=Path.GetFullPath(destination);if(File.Exists(destination))throw new IOException("Destination already exists.");
 var temp=destination+".droidlab-"+Guid.NewGuid().ToString("N")+".partial";long total=0;using var hash=IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
 try{await using(var output=new FileStream(temp,FileMode.CreateNew,FileAccess.Write,FileShare.None,65536,true)){var buffer=new byte[65536];int n;while((n=await source.ReadAsync(buffer.AsMemory(),cancellationToken))>0){total+=n;if(total>expectedBytes)throw new IOException("Source exceeds expected size.");hash.AppendData(buffer,0,n);await output.WriteAsync(buffer.AsMemory(0,n),cancellationToken);progress?.Report(total);}await output.FlushAsync(cancellationToken);}
 if(total!=expectedBytes)throw new IOException("Source size mismatch.");var expected=hash.GetHashAndReset();await using(var check=File.OpenRead(temp)){var actual=await SHA256.HashDataAsync(check,cancellationToken);if(!CryptographicOperations.FixedTimeEquals(expected,actual))throw new IOException("Destination hash mismatch.");}cancellationToken.ThrowIfCancellationRequested();File.Move(temp,destination,false);return new(total,Convert.ToHexString(expected),destination);
 }finally{if(File.Exists(temp))File.Delete(temp);}
 }
}