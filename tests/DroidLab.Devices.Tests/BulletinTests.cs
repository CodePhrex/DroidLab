using DroidLab.Devices;
using Xunit;
public class BulletinTests {
 static DeviceIdentity Identity(string version,string patch)=>new("OEM","Model","Product",version,patch,"","green");
 static BulletinEntry Entry=>new("CVE-2025-12345","2025-06-05",new[]{"15"},"https://source.android.com/docs/security/bulletin/2025-06-01");
 [Theory]
 [InlineData("15","2025-06-01","Potential exposure")]
 [InlineData("15","2025-06-05","Reported patch coverage")]
 [InlineData("14","2025-06-01","Unknown")]
 [InlineData("15","invalid","Unknown")]
 [InlineData("15","2027-01-01","Unknown")]
 public void Correlates(string version,string patch,string expected)=>Assert.Equal(expected,Assert.Single(BulletinCorrelation.Assess(Identity(version,patch),new[]{Entry},new DateOnly(2026,1,1))).Status);
 [Fact] public void ParsesValidRecord()=>Assert.Single(BulletinCorrelation.Parse(System.Text.Json.JsonSerializer.Serialize(new[]{Entry})));
 [Fact] public void RejectsUntrustedSource()=>Assert.Throws<InvalidDataException>(()=>BulletinCorrelation.Parse(System.Text.Json.JsonSerializer.Serialize(new[]{Entry with{Source="https://example.com"}})));
}