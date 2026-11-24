using Xunit;
using DroidLab.Devices;
public class BackupManifestTests {
 [Fact] public async Task InventoriesLocalFiles(){var root=Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString());Directory.CreateDirectory(root);try{await File.WriteAllTextAsync(Path.Combine(root,"sample.txt"),"abc");var entries=await BackupManifest.ScanAsync(root);var entry=Assert.Single(entries);Assert.Equal("sample.txt",entry.RelativePath);Assert.Equal(3L,entry.Bytes);Assert.Equal("BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD",entry.Sha256);}finally{Directory.Delete(root,true);}}
}