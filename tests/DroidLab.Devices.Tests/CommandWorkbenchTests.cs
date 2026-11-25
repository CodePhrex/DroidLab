using DroidLab.Devices;
using Xunit;
public class CommandWorkbenchTests {
 static Device Adb=>new("abc","device","ADB","");
 static Device Fast=>new("xyz","fastboot","Fastboot","");
 [Fact] public void Search()=>Assert.Single(CommandWorkbench.Catalog("ADB","uptime"));
 [Fact] public void ExplicitTarget()=>Assert.Equal(new[]{"-s","xyz","getvar","product"},CommandWorkbench.Arguments(Fast,"product"));
 [Fact] public void RejectCrossTransport()=>Assert.Throws<ArgumentException>(()=>CommandWorkbench.Arguments(Adb,"product"));
 [Fact] public void RejectUnknown()=>Assert.Throws<ArgumentException>(()=>CommandWorkbench.Arguments(Fast,"erase"));
 [Fact] public void RejectUnauthorized()=>Assert.Throws<InvalidOperationException>(()=>CommandWorkbench.Arguments(Adb with {State="unauthorized"},"battery"));
 [Fact] public void BoundedLogcat()=>Assert.Equal(new[]{"-s","abc","logcat","-d","-t","200","*:W"},CommandWorkbench.Arguments(Adb,"logcat"));
 [Fact] public void RejectInvalidSerial()=>Assert.Throws<ArgumentException>(()=>CommandWorkbench.Arguments(Fast with {Serial="x\n"},"slot"));
}