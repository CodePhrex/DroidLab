using DroidLab.Devices;
using Xunit;
public class DiagnosticMetersTests{
 [Fact] public void ParsesRealValues(){var m=DiagnosticMeters.Parse("level: 40\nscale: 50","MemTotal: 1000 kB\nMemAvailable: 250 kB","/dev/data 10G 6G 4G 60% /data");Assert.Equal(80d,m.Battery);Assert.Equal(75d,m.MemoryUsed);Assert.Equal(60d,m.StorageUsed);}
 [Fact] public void MissingIsUnknown()=>Assert.Equal(new DiagnosticMeters(null,null,null),DiagnosticMeters.Parse("","",""));
 [Fact] public void InvalidIsUnknown()=>Assert.Equal(new DiagnosticMeters(null,null,null),DiagnosticMeters.Parse("level: 101\nscale: 100","MemTotal: 0\nMemAvailable: 2","x 1 1 0 101% /data"));
 [Fact] public void ZeroIsValid()=>Assert.Equal(0d,DiagnosticMeters.Parse("level: 0\nscale: 100","","").Battery);
 [Theory][InlineData("ADB","device",true)][InlineData("ADB","unauthorized",false)][InlineData("ADB","offline",false)][InlineData("Fastboot","fastboot",false)] public void ReadGate(string transport,string state,bool expected)=>Assert.Equal(expected,DiagnosticMeters.CanRead(new Device("a",state,transport,"")));
}