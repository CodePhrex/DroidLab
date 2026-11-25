using DroidLab.Devices;
using Xunit;
public class DiscoverySummaryTests {
 [Fact] public void CountsAuthorized()=>Assert.Equal(1,DiscoverySummary.From(new[]{new DiscoveryResult("ADB",new[]{new Device("a","device","ADB","")},null)}).AuthorizedAdb);
 [Fact] public void CountsFastboot()=>Assert.Equal(1,DiscoverySummary.From(new[]{new DiscoveryResult("Fastboot",new[]{new Device("a","fastboot","Fastboot","")},null)}).Fastboot);
 [Fact] public void UnauthorizedIsOther()=>Assert.Equal(1,DiscoverySummary.From(new[]{new DiscoveryResult("ADB",new[]{new Device("a","unauthorized","ADB","")},null)}).Other);
 [Fact] public void FailureNotZeroSuccess()=>Assert.Equal(1,DiscoverySummary.From(new[]{new DiscoveryResult("ADB",Array.Empty<Device>(),"missing")}).FailedTransports);
 [Fact] public void EmptySnapshot()=>Assert.Equal(new DiscoverySummary(0,0,0,0),DiscoverySummary.From(Array.Empty<DiscoveryResult>()));
}