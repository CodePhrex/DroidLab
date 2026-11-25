using System.Text.RegularExpressions;
namespace DroidLab.Devices;
public static class PartitionDiagnostics{
 public static bool ValidName(string name)=>Regex.IsMatch(name,@"\A[A-Za-z0-9_]{1,64}\z");
 public static ulong? ParseSize(string value){var m=Regex.Match(value,@"(?im)^\s*(?:\(bootloader\)\s*)?partition-size:[A-Za-z0-9_]+:\s*0x([0-9a-f]+)\s*$");return m.Success&&ulong.TryParse(m.Groups[1].Value,System.Globalization.NumberStyles.HexNumber,null,out var size)?size:null;}
}