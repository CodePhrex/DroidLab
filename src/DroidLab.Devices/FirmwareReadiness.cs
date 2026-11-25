namespace DroidLab.Devices;
public record FirmwareTarget(string Manufacturer,string Model,string Product,string Sha256,long? RollbackIndex);
public record FirmwareReadinessResult(string PackageType,IReadOnlyList<SecurityFinding> Findings,bool FlashApproved);
public static class FirmwareReadiness {
 static bool Match(string a,string b)=>!string.IsNullOrWhiteSpace(a)&&!string.IsNullOrWhiteSpace(b)&&string.Equals(a.Trim(),b.Trim(),StringComparison.OrdinalIgnoreCase);
 public static string Classify(FirmwareInspection file){
 var ext=Path.GetExtension(file.Name).ToLowerInvariant();
 if(ext==".zip"){
 if(file.Entries.Contains("payload.bin",StringComparer.Ordinal))return "OTA payload candidate";
 if(file.Entries.Contains("android-info.txt",StringComparer.Ordinal))return "Fastboot archive candidate";
 return "Unclassified ZIP";
 }
 return ext switch{".tar" or ".md5"=>"OEM archive candidate",".img"=>"Raw image candidate",".bin"=>"Binary candidate",_=>"Unknown"};
 }
 public static FirmwareReadinessResult Assess(DeviceIdentity device,FirmwareInspection file,FirmwareTarget target,long? deviceRollback){
 var findings=new List<SecurityFinding>();
 foreach(var (name,actual,expected) in new[]{("Manufacturer",device.Manufacturer,target.Manufacturer),("Model",device.Model,target.Model),("Product",device.Product,target.Product)})
 findings.Add(new(name,string.IsNullOrWhiteSpace(actual)||string.IsNullOrWhiteSpace(expected)?"Unknown":Match(actual,expected)?"Declared match":"Mismatch",$"Device: {actual}; declared target: {expected}"));
 var hashValid=target.Sha256 is {Length:64}&&target.Sha256.All(Uri.IsHexDigit);
 findings.Add(new("SHA256",!hashValid?"Unknown":Match(file.Sha256,target.Sha256)?"Declared match":"Mismatch","Checksum comparison does not authenticate firmware."));
 findings.Add(new("Rollback",deviceRollback is null||target.RollbackIndex is null||deviceRollback<0||target.RollbackIndex<0?"Unknown":target.RollbackIndex<deviceRollback?"Downgrade risk":"Declared index not lower","Indices must come from comparable OEM rollback domains; no automatic validation."));
 findings.Add(new("Bootloader","Unknown","OEM bootloader, carrier variant, signature and authorization checks remain required."));
 return new(Classify(file),findings,false);
 }
}