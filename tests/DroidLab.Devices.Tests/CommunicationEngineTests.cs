using DroidLab.Devices;
using Xunit;
public class CommunicationEngineTests{
 sealed class Fake:IDeviceTransport{
 public string Name=>"ADB";
 public DiscoveryResult Result=new("ADB",[],null);
 public Task<DiscoveryResult> ScanAsync(CancellationToken token)=>Task.FromResult(Result);
 }
 static Device Phone=>new("serial","device","ADB","");
 [Fact] public async Task TracksArrivalAndDeparture(){var f=new Fake();var engine=new CommunicationEngine(new[]{f});f.Result=new("ADB",new[]{Phone},null);var a=await engine.ScanAsync();Assert.Single(a.Connected);var b=await engine.ScanAsync();Assert.Empty(b.Connected);Assert.Equal(a.Sessions[0].FirstSeen,b.Sessions[0].FirstSeen);f.Result=new("ADB",[],null);Assert.Single((await engine.ScanAsync()).Disconnected);}
 [Fact] public async Task FailurePreservesStaleSession(){var f=new Fake{Result=new("ADB",new[]{Phone},null)};var e=new CommunicationEngine(new[]{f});await e.ScanAsync();f.Result=new("ADB",[],"tool missing");var s=await e.ScanAsync();Assert.True(Assert.Single(s.Sessions).Stale);Assert.Empty(s.Disconnected);}
 [Fact] public async Task RecoveryClearsStale(){var f=new Fake{Result=new("ADB",new[]{Phone},null)};var e=new CommunicationEngine(new[]{f});await e.ScanAsync();f.Result=new("ADB",[],"offline");await e.ScanAsync();f.Result=new("ADB",new[]{Phone},null);var s=await e.ScanAsync();Assert.False(Assert.Single(s.Sessions).Stale);Assert.Empty(s.Connected);}
 [Fact] public async Task DeduplicatesTransportSerial(){var f=new Fake{Result=new("ADB",new[]{Phone,Phone},null)};Assert.Single((await new CommunicationEngine(new[]{f}).ScanAsync()).Sessions);}
 [Fact] public void RejectsDuplicateAdapters()=>Assert.Throws<ArgumentException>(()=>new CommunicationEngine(new[]{new Fake(),new Fake()}));
 [Fact] public async Task CancellationDoesNotScan(){var e=new CommunicationEngine(new[]{new Fake()});await Assert.ThrowsAnyAsync<OperationCanceledException>(()=>e.ScanAsync(new CancellationToken(true)));}
}