using Xunit;
using DroidLab.Devices;
public class SharedStorageBackupTests {
 [Fact] public void TargetsSelectedDevice(){Assert.Equal(new[]{"-s","abc","pull","/sdcard/DCIM","destination"},SharedStorageBackup.Arguments(new("abc","device","ADB",""),BackupFolder.DCIM,"destination"));}
 [Fact] public void RejectUnauthorized(){Assert.Throws<InvalidOperationException>(()=>SharedStorageBackup.Arguments(new("abc","unauthorized","ADB",""),BackupFolder.DCIM,"destination"));}
 [Fact] public void RejectInvalidFolder(){Assert.Throws<ArgumentOutOfRangeException>(()=>SharedStorageBackup.Arguments(new("abc","device","ADB",""),(BackupFolder)99,"destination"));}
 [Fact] public void FastbootDoesNotImplyCompatibility(){Assert.Contains("unverified",RecoveryReadiness.Describe(new("abc","fastboot","Fastboot","")));}
}