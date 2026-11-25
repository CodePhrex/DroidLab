namespace DroidLab.Devices;
public record RecoveryPlan(bool Ready,string Guidance);
public record RecoverySession(string Serial,RebootTarget Target,DateTimeOffset Started,string Status,string Detail);
public static class GuidedRecovery {
 public static RecoveryPlan Plan(Device device,DeviceIdentity identity,int? charge,bool backupAcknowledged,RebootTarget target){
 if(device.Transport!="ADB"||device.State!="device")return new(false,"Authorized online ADB required.");
 if(!Enum.IsDefined(target))return new(false,"Unsupported target.");
 if(string.IsNullOrWhiteSpace(identity.Manufacturer)||string.IsNullOrWhiteSpace(identity.Model))return new(false,"Device identity unavailable.");
 if(charge is null or <50 or >100)return new(false,"Confirm reported battery charge of at least 50%.");
 if(!backupAcknowledged)return new(false,"Acknowledge backup and recovery risks.");
 var oem=OemCompatibility.Identify(identity.Manufacturer);
 if(target==RebootTarget.Bootloader&&!oem.SuggestedTransport.StartsWith("Fastboot",StringComparison.Ordinal))return new(false,"Bootloader transition is OEM-specific: "+oem.SuggestedTransport);
 return new(true,"Reboot only; no flashing or reset. "+oem.SuggestedTransport);
 }
 public static bool Observed(string serial,RebootTarget target,IEnumerable<Device> devices)=>devices.Any(d=>d.Serial==serial&&(target switch{RebootTarget.System=>d.Transport=="ADB"&&d.State=="device",RebootTarget.Recovery=>d.Transport=="ADB"&&d.State=="recovery",RebootTarget.Bootloader=>d.Transport=="Fastboot"&&d.State=="fastboot",_=>false}));
 public static int? Charge(string raw){
 var fields=raw.Split('\n').Select(x=>x.Trim().Split(':',2)).Where(x=>x.Length==2).ToList();
 int? Get(string key)=>int.TryParse(fields.FirstOrDefault(x=>x[0]==key)?[1],out var n)?n:null;
 var level=Get("level");var scale=Get("scale");return level is >=0&&scale is >0&&level<=scale?(int)(100L*level.Value/scale.Value):null;
 }
 public static async Task<RecoverySession> ExecuteAsync(Device device,RebootTarget target,bool backupAcknowledged,CancellationToken token=default){
 var started=DateTimeOffset.UtcNow;
 var scan=await Discovery.ScanAsync("ADB",token);
 if(scan.Error!=null||!scan.Devices.Any(d=>d.Serial==device.Serial&&d.State=="device"))return new(device.Serial,target,started,"Blocked","Device no longer authorized online.");
 var identity=IdentityParser.Parse(await Diagnostics.ReadAsync(device,DiagnosticKind.Identity));
 var charge=Charge(await Diagnostics.ReadAsync(device,DiagnosticKind.Battery));
 var plan=Plan(device,identity,charge,backupAcknowledged,target);
 if(!plan.Ready)return new(device.Serial,target,started,"Blocked",plan.Guidance);
 await DeviceReboot.RequestAsync(device,target,token);
 bool absent=false;
 for(int i=0;i<8;i++){
 await Task.Delay(TimeSpan.FromSeconds(3),token);
 var result=await Discovery.ScanAsync(target==RebootTarget.Bootloader?"Fastboot":"ADB",token);
 if(result.Error!=null)continue;
 if(!Observed(device.Serial,target,result.Devices)){absent=true;continue;}
 if(target!=RebootTarget.System||absent)return new(device.Serial,target,started,"Target observed","Matching serial and expected mode observed; no functional health guarantee.");
 }
 return new(device.Serial,target,started,"Unverified","Reboot requested; expected transition not confirmed. Inspect device manually; do not automatically retry.");
 }
}