using DroidLab.Devices;
using Xunit;
public class FirmwareReadinessTests {
 static DeviceIdentity Device=>new("Samsung","SM-X","product","15","","","green");
 static FirmwareInspection File=>new("package.zip",1,new string('A',64),null,new[]{"payload.bin"});
 static FirmwareTarget Target=>new("Samsung","SM-X","product",new string('A',64),5);
 [Fact] public void NeverApprovesFlash()=>Assert.False(FirmwareReadiness.Assess(Device,File,Target,4).FlashApproved);
 [Fact] public void DetectsModelMismatch()=>Assert.Contains(FirmwareReadiness.Assess(Device,File,Target with{Model="other"},4).Findings,x=>x.Code=="Model"&&x.Status=="Mismatch");
 [Fact] public void DetectsRollback()=>Assert.Contains(FirmwareReadiness.Assess(Device,File,Target,6).Findings,x=>x.Code=="Rollback"&&x.Status=="Downgrade risk");
 [Fact] public void UnknownRollback()=>Assert.Contains(FirmwareReadiness.Assess(Device,File,Target,null).Findings,x=>x.Code=="Rollback"&&x.Status=="Unknown");
 [Fact] public void DetectsHashMismatch()=>Assert.Contains(FirmwareReadiness.Assess(Device,File,Target with{Sha256=new string('B',64)},4).Findings,x=>x.Code=="SHA256"&&x.Status=="Mismatch");
 [Fact] public void ClassifiesPayload()=>Assert.Equal("OTA payload candidate",FirmwareReadiness.Classify(File));
 [Fact] public void MissingProductUnknown()=>Assert.Contains(FirmwareReadiness.Assess(Device,File,Target with{Product=""},4).Findings,x=>x.Code=="Product"&&x.Status=="Unknown");
}