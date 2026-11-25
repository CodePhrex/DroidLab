using DroidLab.Devices;
using Xunit;
public class UsbIdentityTests{
 [Theory][InlineData(@"USB\VID_04e8&PID_6860\abc","04E8","6860","Samsung")][InlineData(@"USB\VID_18D1&PID_4EE7&MI_01\abc","18D1","4EE7","Google")][InlineData(@"USB\VID_FFFF&PID_0001\abc","FFFF","0001","Unknown vendor")] public void Parses(string id,string vid,string pid,string vendor)=>Assert.Equal(new UsbIdentity(vid,pid,vendor),UsbIdentity.Parse(id));
 [Theory][InlineData(@"PCI\VID_04E8&PID_6860")][InlineData(@"USB\VID_123&PID_0001")][InlineData(@"USB\ROOT_HUB30")] public void Rejects(string id)=>Assert.Null(UsbIdentity.Parse(id));
 [Fact] public async Task RejectsUnknownTool()=>await Assert.ThrowsAsync<ArgumentException>(()=>PlatformToolHealth.CheckAsync("other"));
}