using Xunit;
using DroidLab.Devices;
public class DiscoveryTests {
 [Fact] public void AdbStates(){var d=Discovery.Parse("List of devices attached\r\na device model:Pixel\r\nb unauthorized\r\nc offline\r\n","ADB");Assert.Equal(3,d.Count);Assert.Equal("unauthorized",d[1].State);Assert.Equal("model:Pixel",d[0].Details);}
 [Fact] public void Fastboot(){var d=Discovery.Parse("abc\tfastboot\nnoise line","Fastboot");Assert.Single(d);Assert.Equal("abc",d[0].Serial);}
 [Fact] public void Empty(){Assert.Empty(Discovery.Parse("List of devices attached\n","ADB"));}
 [Fact] public void RejectUnknownTransport(){Assert.Throws<ArgumentException>(()=>Discovery.Parse("","unknown"));}
}