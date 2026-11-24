using System.Globalization;
namespace DroidLab.Devices;
public record SecurityFinding(string Code,string Status,string Detail);
public static class SecurityAssessment {
 public static IReadOnlyList<SecurityFinding> Assess(DeviceIdentity identity,DateOnly today){
 var findings=new List<SecurityFinding>();
 if(!DateOnly.TryParseExact(identity.SecurityPatch,"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None,out var patch)||patch>today) findings.Add(new("PATCH","Unknown","Missing, invalid or future patch date."));
 else {var age=today.DayNumber-patch.DayNumber;findings.Add(new("PATCH",age>90?"Review":"Observed",$"Reported patch: {patch:yyyy-MM-dd}; age {age} days. The 90-day review threshold is a heuristic, not an OEM support or CVE determination."));}
 findings.Add(new("BOOT",identity.BootState=="green"?"Observed":string.IsNullOrWhiteSpace(identity.BootState)?"Unknown":"Review","Reported verified boot state: "+(string.IsNullOrWhiteSpace(identity.BootState)?"unavailable":identity.BootState)));
 findings.Add(new("CVE","Not assessed","No advisory correlation performed. No vulnerability or exploitation is confirmed."));
 return findings;
 }
}