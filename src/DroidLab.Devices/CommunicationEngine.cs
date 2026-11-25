namespace DroidLab.Devices;
public interface IDeviceTransport {
 string Name {get;}
 Task<DiscoveryResult> ScanAsync(CancellationToken cancellationToken);
}
public sealed class PlatformTransport(string name):IDeviceTransport{
 public string Name {get;}=name is "ADB" or "Fastboot"?name:throw new ArgumentException("Unsupported transport");
 public Task<DiscoveryResult> ScanAsync(CancellationToken cancellationToken)=>Discovery.ScanAsync(Name,cancellationToken);
}
public record DeviceSession(Device Device,DateTimeOffset FirstSeen,DateTimeOffset LastSeen,bool Stale);
public record CommunicationSnapshot(DateTimeOffset CapturedAt,IReadOnlyList<DeviceSession> Sessions,IReadOnlyList<Device> Connected,IReadOnlyList<Device> Disconnected,IReadOnlyList<DiscoveryResult> Results);
public sealed class CommunicationEngine{
 readonly IDeviceTransport[] transports;
 readonly SemaphoreSlim gate=new(1,1);
 Dictionary<(string,string),DeviceSession> sessions=new();
 public CommunicationEngine(IEnumerable<IDeviceTransport> adapters){transports=adapters.ToArray();if(transports.Select(x=>x.Name).Distinct(StringComparer.Ordinal).Count()!=transports.Length)throw new ArgumentException("Duplicate transport");}
 public static CommunicationEngine Default()=>new(new IDeviceTransport[]{new PlatformTransport("ADB"),new PlatformTransport("Fastboot")});
 public async Task<CommunicationSnapshot> ScanAsync(CancellationToken token=default){
 await gate.WaitAsync(token);try{
 async Task<DiscoveryResult> Scan(IDeviceTransport adapter){try{var result=await adapter.ScanAsync(token);return result.Transport==adapter.Name?result:new(adapter.Name,[],"Transport mismatch");}catch(OperationCanceledException) when(token.IsCancellationRequested){throw;}catch(Exception ex){return new(adapter.Name,[],ex.Message);}}
 var results=await Task.WhenAll(transports.Select(Scan));token.ThrowIfCancellationRequested();
 var now=DateTimeOffset.UtcNow;var next=new Dictionary<(string,string),DeviceSession>();var connected=new List<Device>();var disconnected=new List<Device>();
 foreach(var result in results){
 if(result.Error!=null){foreach(var pair in sessions.Where(p=>p.Key.Item1==result.Transport))next[pair.Key]=pair.Value with {Stale=true};continue;}
 foreach(var device in result.Devices.Where(d=>d.Transport==result.Transport)){
 var key=(device.Transport,device.Serial);if(next.ContainsKey(key))continue;
 if(sessions.TryGetValue(key,out var old))next[key]=new(device,old.FirstSeen,now,false);else{next[key]=new(device,now,now,false);connected.Add(device);}
 }
 foreach(var pair in sessions.Where(p=>p.Key.Item1==result.Transport&&!next.ContainsKey(p.Key)))disconnected.Add(pair.Value.Device);
 }
 sessions=next;return new(now,next.Values.ToArray(),connected.ToArray(),disconnected.ToArray(),results);
 }finally{gate.Release();}
 }
}