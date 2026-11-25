using DroidLab.Devices;
using Xunit;
public class SecurityRegressionTests {
 [Fact] public void GateRejectsOverlap(){var gate=new OperationGate();using var first=gate.TryEnter();Assert.NotNull(first);Assert.Null(gate.TryEnter());}
 [Fact] public void GateReleases(){var gate=new OperationGate();gate.TryEnter()!.Dispose();using var next=gate.TryEnter();Assert.NotNull(next);}
 [Fact] public void DoubleDisposeSafe(){var gate=new OperationGate();var first=gate.TryEnter()!;first.Dispose();using var next=gate.TryEnter();first.Dispose();Assert.Null(gate.TryEnter());}
 [Theory][InlineData("shell","reboot")][InlineData("shell","settings","put","global","wifi_on","1")][InlineData("shell","ls","-la","/sdcard/../data")][InlineData("uninstall","com.test;id")][InlineData("shell","getprop","ro.serialno")]
 public void RejectsUnexpectedCommands(params string[] args)=>Assert.False(ApplicationStorage.IsAllowed(args));
 [Fact] public void AllowsNetworkQueries()=>Assert.All(NetworkDiagnostics.Queries(),q=>Assert.True(ApplicationStorage.IsAllowed(q.Arguments)));
 [Fact] public void AllowsPackageMetadata()=>Assert.True(ApplicationStorage.IsAllowed(ApplicationStorage.PackageArguments("com.example.app")));
 [Fact] public void AllowsSharedListing()=>Assert.True(ApplicationStorage.IsAllowed(new[]{"shell","ls","-la","/sdcard/Download"}));
 [Fact] public void RejectsNullLicenseFields()=>Assert.False(Licensing.Validate("{\"Payload\":null,\"Signature\":null}","invalid",DateTimeOffset.UtcNow).Valid);
}