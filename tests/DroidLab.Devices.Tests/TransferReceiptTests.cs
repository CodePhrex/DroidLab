using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DroidLab.Devices;
using Xunit;
public class TransferReceiptTests{
 [Fact]public async Task ReceiptMatchesBytes(){var root=Directory.CreateTempSubdirectory();try{var bytes=new byte[]{7,8,9};var dest=Path.Combine(root.FullName,"receipt.bin");using var stream=new MemoryStream(bytes);var receipt=await VerifiedTransfer.CopyAsync(stream,dest,bytes.Length);Assert.Equal(Convert.ToHexString(SHA256.HashData(bytes)),receipt.Sha256);Assert.Equal(dest,receipt.Destination);Assert.Equal(3L,receipt.Bytes);}finally{root.Delete(true);}}
}