using Xunit;
using DroidLab.Devices;
public class DiagnosticsTests {
 [Theory]
 [InlineData(DiagnosticKind.Battery,"dumpsys")]
 [InlineData(DiagnosticKind.Storage,"df")]
 [InlineData(DiagnosticKind.Identity,"getprop")]
 [InlineData(DiagnosticKind.Memory,"cat")]
 [InlineData(DiagnosticKind.Applications,"pm")]
 public void CommandsTargetSelectedDevice(DiagnosticKind kind,string command){var args=Diagnostics.Arguments("serial123",kind);Assert.Equal("-s",args[0]);Assert.Equal("serial123",args[1]);Assert.Equal("shell",args[2]);Assert.Equal(command,args[3]);}
 [Fact] public void RejectEmptySerial()=>Assert.Throws<ArgumentException>(()=>Diagnostics.Arguments("",DiagnosticKind.Battery));
 [Fact] public async Task RejectUnauthorized()=>await Assert.ThrowsAsync<InvalidOperationException>(()=>Diagnostics.ReadAsync(new Device("abc","unauthorized","ADB",""),DiagnosticKind.Battery));
}