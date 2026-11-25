using System.Text.RegularExpressions;
namespace DroidLab.Devices;
public record UsbIdentity(string VendorId,string ProductId,string VendorHint){
 public static UsbIdentity? Parse(string id){var m=Regex.Match(id,@"^USB\\VID_([0-9A-F]{4})&PID_([0-9A-F]{4})(?:[&\\]|$)",RegexOptions.IgnoreCase|RegexOptions.CultureInvariant);if(!m.Success)return null;var vid=m.Groups[1].Value.ToUpperInvariant();return new(vid,m.Groups[2].Value.ToUpperInvariant(),vid switch{"04E8"=>"Samsung","18D1"=>"Google","22B8"=>"Motorola","2717"=>"Xiaomi","12D1"=>"Huawei","0BB4"=>"HTC","05C6"=>"Qualcomm","0E8D"=>"MediaTek",_=>"Unknown vendor"});}
}