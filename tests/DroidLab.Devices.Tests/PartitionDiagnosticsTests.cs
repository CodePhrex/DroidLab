using DroidLab.Devices;
using Xunit;
public class PartitionDiagnosticsTests{
 [Theory][InlineData("system_a",true)][InlineData("boot",true)][InlineData("../boot",false)][InlineData("boot;erase",false)][InlineData("",false)]public void Names(string name,bool expected)=>Assert.Equal(expected,PartitionDiagnostics.ValidName(name));
 [Fact]public void Size()=>Assert.Equal(4096UL,PartitionDiagnostics.ParseSize("(bootloader) partition-size:boot: 0x1000"));
 [Fact]public void Unknown()=>Assert.Null(PartitionDiagnostics.ParseSize("FAILED"));
}