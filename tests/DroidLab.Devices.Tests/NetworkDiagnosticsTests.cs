using DroidLab.Devices;
using Xunit;
public class NetworkDiagnosticsTests {
 [Theory][InlineData("1","Enabled")][InlineData("0","Disabled")][InlineData("null","Unknown")][InlineData("2","Unknown")] public void Settings(string raw,string expected)=>Assert.Equal(expected,NetworkDiagnostics.Interpret("Wi-Fi setting",raw).Value);
 [Fact] public void DualSim()=>Assert.Equal("READY,ABSENT",NetworkDiagnostics.Interpret("SIM state","READY,ABSENT\n").Value);
 [Fact] public void EmptyUnknown()=>Assert.Equal("Unknown",NetworkDiagnostics.Interpret("Carrier","").Value);
 [Fact] public void NoSubscriberQueries()=>Assert.All(NetworkDiagnostics.Queries(),q=>Assert.DoesNotContain(q.Arguments,a=>a.Contains("imei",StringComparison.OrdinalIgnoreCase)||a.Contains("imsi",StringComparison.OrdinalIgnoreCase)||a.Contains("iccid",StringComparison.OrdinalIgnoreCase)));
}