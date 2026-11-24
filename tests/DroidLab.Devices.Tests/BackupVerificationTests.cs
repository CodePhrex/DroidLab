using Xunit;
using DroidLab.Devices;
public class BackupVerificationTests {
 [Fact] public async Task DetectsChangedAndUnexpectedFiles(){var root=Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString());var data=Directory.CreateDirectory(Path.Combine(root,"data")).FullName;try{var path=Path.Combine(data,"a.txt");await File.WriteAllTextAsync(path,"abc");await File.WriteAllTextAsync(Path.Combine(root,"manifest.json"),BackupManifest.Serialize(await BackupManifest.ScanAsync(data)));Assert.True((await BackupVerification.VerifyAsync(root)).Valid);await File.WriteAllTextAsync(path,"xyz");await File.WriteAllTextAsync(Path.Combine(data,"b.txt"),"new");var result=await BackupVerification.VerifyAsync(root);Assert.False(result.Valid);Assert.Equal(2,result.Issues.Count);}finally{Directory.Delete(root,true);}}
 [Fact] public void RebootTargetsSelectedDevice(){Assert.Equal(new[]{"-s","abc","reboot","recovery"},DeviceReboot.Arguments(new("abc","device","ADB",""),RebootTarget.Recovery));}
 [Fact] public void RebootRejectsUnauthorized(){Assert.Throws<InvalidOperationException>(()=>DeviceReboot.Arguments(new("abc","unauthorized","ADB",""),RebootTarget.System));}
 [Fact] public void OemIdentificationNeverApprovesFlash(){Assert.False(OemCompatibility.Identify("Samsung").FlashApproved);}
}