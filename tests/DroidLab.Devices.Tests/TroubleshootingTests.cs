using DroidLab.Devices;
using Xunit;
public class TroubleshootingTests {
 [Fact] public void LowCharge()=>Assert.Contains(Troubleshooting.Assess("level: 5\nscale: 100","",""),x=>x.Category=="Battery charge"&&x.Severity=="High");
 [Fact] public void HotBattery()=>Assert.Contains(Troubleshooting.Assess("temperature: 510","",""),x=>x.Category=="Battery temperature"&&x.Severity=="High");
 [Fact] public void FullData()=>Assert.Contains(Troubleshooting.Assess("","/dev/block 10G 9G 1G 96% /data",""),x=>x.Category=="Data storage"&&x.Severity=="High");
 [Fact] public void LowMemory()=>Assert.Contains(Troubleshooting.Assess("","","MemTotal: 1000 kB\nMemAvailable: 40 kB"),x=>x.Category=="Available memory"&&x.Severity=="High");
 [Fact] public void MissingDataUnknown()=>Assert.All(Troubleshooting.Assess("","",""),x=>Assert.Equal("Unknown",x.Severity));
 [Fact] public void InvalidScaleUnknown()=>Assert.Contains(Troubleshooting.Assess("level: 100\nscale: 0","",""),x=>x.Category=="Battery charge"&&x.Severity=="Unknown");
 [Fact] public void ReportedHealthGood()=>Assert.Contains(Troubleshooting.Assess("health: 2","",""),x=>x.Category=="Battery health"&&x.Severity=="Observed");
}