using DroidLab.Devices;
using Xunit;
public class RestoreTests {
 [Fact] public void UniqueDestination(){Assert.NotEqual(SharedStorageRestore.Destination(Guid.NewGuid()),SharedStorageRestore.Destination(Guid.NewGuid()));}
 [Fact] public void RejectUnauthorized(){Assert.Throws<InvalidOperationException>(()=>SharedStorageRestore.PushArguments(new("serial","unauthorized","ADB",""),"data",SharedStorageRestore.Destination(Guid.NewGuid())));}
 [Fact] public void RejectOriginalFolder(){Assert.Throws<ArgumentException>(()=>SharedStorageRestore.PushArguments(new("serial","device","ADB",""),"data","/sdcard/DCIM"));}
 [Fact] public void ExplicitSerialAndStaging(){var d=SharedStorageRestore.Destination(Guid.NewGuid());Assert.Equal(new[]{"-s","serial","push","data",d+"/"},SharedStorageRestore.PushArguments(new("serial","device","ADB",""),"data",d));}
}