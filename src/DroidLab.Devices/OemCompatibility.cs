namespace DroidLab.Devices;
public record OemProfile(string Manufacturer,string SuggestedTransport,bool FlashApproved);
public static class OemCompatibility {
 public static OemProfile Identify(string manufacturer){
 var name=manufacturer.Trim().ToLowerInvariant();
 return name switch {"samsung"=>new("Samsung","Download Mode (OEM-specific)",false),"google"=>new("Google","Fastboot / Fastbootd",false),"motorola"=>new("Motorola","Fastboot (model dependent)",false),"oneplus"=>new("OnePlus","Fastboot (model dependent)",false),"xiaomi"=>new("Xiaomi","Fastboot (model dependent)",false),_=>new(manufacturer,"Unknown; consult OEM documentation",false)};
 }
}