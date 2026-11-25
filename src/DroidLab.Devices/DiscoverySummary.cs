namespace DroidLab.Devices;
public record DiscoverySummary(int AuthorizedAdb,int Fastboot,int Other,int FailedTransports){
 public static DiscoverySummary From(IEnumerable<DiscoveryResult> results){
 var all=results.ToArray();var devices=all.Where(r=>r.Error==null).SelectMany(r=>r.Devices).ToArray();
 return new(devices.Count(d=>d.Transport=="ADB"&&d.State=="device"),devices.Count(d=>d.Transport=="Fastboot"&&d.State=="fastboot"),devices.Count(d=>!(d.Transport=="ADB"&&d.State=="device")&&!(d.Transport=="Fastboot"&&d.State=="fastboot")),all.Count(r=>r.Error!=null));
 }
}