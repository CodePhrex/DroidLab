namespace DroidLab.Devices;
public record NetworkObservation(string Name,string Value,string Note);
public static class NetworkDiagnostics {
 public static IReadOnlyList<(string Name,string[] Arguments)> Queries()=>new (string,string[])[]{
 ("SIM state",new[]{"shell","getprop","gsm.sim.state"}),
 ("Carrier",new[]{"shell","getprop","gsm.operator.alpha"}),
 ("Radio type",new[]{"shell","getprop","gsm.network.type"}),
 ("Wi-Fi setting",new[]{"shell","settings","get","global","wifi_on"}),
 ("Bluetooth setting",new[]{"shell","settings","get","global","bluetooth_on"}),
 ("Airplane mode",new[]{"shell","settings","get","global","airplane_mode_on"})};
 public static NetworkObservation Interpret(string name,string raw){
 var value=raw.Trim();if(value.Length==0||value=="null"||value.Length>256||value.Any(char.IsControl))return new(name,"Unknown","Reading unavailable or unsupported.");
 if(name.EndsWith("setting",StringComparison.Ordinal)||name=="Airplane mode")return new(name,value=="1"?"Enabled":value=="0"?"Disabled":"Unknown","Reported setting; does not establish connection or internet access.");
 return new(name,value,"Reported Android property; may be stale or contain multiple SIM values.");
 }
 public static async Task<IReadOnlyList<NetworkObservation>> ReadAsync(Device device){
 if(device.Transport!="ADB"||device.State!="device")throw new InvalidOperationException("Select an authorized online ADB device.");
 var result=new List<NetworkObservation>();
 foreach(var (name,args) in Queries()){try{result.Add(Interpret(name,await ApplicationStorage.RunAsync(device,args)));}catch(Exception){result.Add(new(name,"Unknown","Query failed or unsupported."));}}
 return result;
 }
}