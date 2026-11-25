using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DroidLab.Devices;
using Xunit;
public class VerifiedTransferTests{
 [Theory][InlineData(3,true)][InlineData(2,false)][InlineData(4,false)]public async Task SizeVerification(int size,bool success){var root=Directory.CreateTempSubdirectory();try{var dest=Path.Combine(root.FullName,"file");using var source=new MemoryStream(new byte[]{1,2,3});if(success){var receipt=await VerifiedTransfer.CopyAsync(source,dest,size);Assert.Equal(3L,receipt.Bytes);Assert.Equal(new byte[]{1,2,3},File.ReadAllBytes(dest));}else{await Assert.ThrowsAsync<IOException>(()=>VerifiedTransfer.CopyAsync(source,dest,size));Assert.Empty(root.GetFiles());}}finally{root.Delete(true);}}
 [Fact]public async Task NoOverwrite(){var root=Directory.CreateTempSubdirectory();try{var dest=Path.Combine(root.FullName,"file");File.WriteAllText(dest,"original");await Assert.ThrowsAsync<IOException>(()=>VerifiedTransfer.CopyAsync(new MemoryStream(new byte[]{1}),dest,1));Assert.Equal("original",File.ReadAllText(dest));}finally{root.Delete(true);}}
 [Fact]public async Task Cancel(){var root=Directory.CreateTempSubdirectory();try{await Assert.ThrowsAnyAsync<OperationCanceledException>(()=>VerifiedTransfer.CopyAsync(new MemoryStream(new byte[]{1}),Path.Combine(root.FullName,"file"),1,cancellationToken:new CancellationToken(true)));Assert.Empty(root.GetFiles());}finally{root.Delete(true);}}
}