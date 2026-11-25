using DroidLab.Devices;
using Xunit;
public class AdvancedProtocolsTests{
 [Theory][InlineData("192.168.1.2:5555",true)][InlineData("192.168.1.2:65536",false)][InlineData("host:5555",false)][InlineData("127.0.0.1:0",false)][InlineData("1.2.3.4:5555;cmd",false)]public void Endpoint(string value,bool valid)=>Assert.Equal(valid,AdvancedProtocols.ValidEndpoint(value));
 [Theory][InlineData("123456",true)][InlineData("12345",false)][InlineData("123456\n",false)]public void PairCode(string value,bool valid)=>Assert.Equal(valid,AdvancedProtocols.ValidPairCode(value));
 [Theory][InlineData("(bootloader) is-userspace: yes","Fastbootd")][InlineData("is-userspace: no","Bootloader Fastboot")][InlineData("unknown","Unknown")]public void Mode(string raw,string expected)=>Assert.Equal(expected,AdvancedProtocols.FastbootMode(raw));
}