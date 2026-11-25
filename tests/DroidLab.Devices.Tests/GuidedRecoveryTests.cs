using DroidLab.Devices;
using Xunit;
public class GuidedRecoveryTests {
 static Device Online=>new("abc","device","ADB","");
 static DeviceIdentity Identity=>IdentityParser.Parse("[ro.product.manufacturer]: [Google]\n[ro.product.model]: [Pixel]");
 [Fact] public void LowChargeBlocks()=>Assert.False(GuidedRecovery.Plan(Online,Identity,20,true,RebootTarget.Recovery).Ready);
 [Fact] public void BackupRequired()=>Assert.False(GuidedRecovery.Plan(Online,Identity,80,false,RebootTarget.Recovery).Ready);
 [Fact] public void ReadyRecovery()=>Assert.True(GuidedRecovery.Plan(Online,Identity,80,true,RebootTarget.Recovery).Ready);
 [Fact] public void DifferentSerialNotVerified()=>Assert.False(GuidedRecovery.Observed("abc",RebootTarget.Recovery,new[]{new Device("xyz","recovery","ADB","")}));
 [Fact] public void FastbootObserved()=>Assert.True(GuidedRecovery.Observed("abc",RebootTarget.Bootloader,new[]{new Device("abc","fastboot","Fastboot","")}));
 [Fact] public void ChargeScale()=>Assert.Equal(75,GuidedRecovery.Charge("level: 150\nscale: 200"));
 [Fact] public void InvalidCharge()=>Assert.Null(GuidedRecovery.Charge("level: 150\nscale: 0"));
}