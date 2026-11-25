using System.Net;
using System.Text.Json;
namespace DroidLab.Devices;
public record ReportRow(string Section,string Item,string Status,string Detail);
public record ProfessionalReport(DateTimeOffset CollectedAt,IReadOnlyList<ReportRow> Rows);
public static class Reporting {
 public static ProfessionalReport Build(DeviceIdentity identity,string battery,string storage,string memory,IEnumerable<string> errors,DateTimeOffset now){
 var rows=new List<ReportRow>();
 foreach(var item in new[]{("Manufacturer",identity.Manufacturer),("Model",identity.Model),("Android",identity.AndroidVersion),("Patch",identity.SecurityPatch)})rows.Add(new("Identity",item.Item1,string.IsNullOrWhiteSpace(item.Item2)?"Unknown":"Reported",item.Item2));
 rows.AddRange(Troubleshooting.Assess(battery,storage,memory).Select(x=>new ReportRow("Diagnostics",x.Category,x.Severity,x.Evidence+" — "+x.Recommendation)));
 rows.AddRange(SecurityAssessment.Assess(identity,DateOnly.FromDateTime(now.UtcDateTime)).Select(x=>new ReportRow("Security",x.Code,x.Status,x.Detail)));
 rows.AddRange(errors.Select(x=>new ReportRow("Collection",x,"Unavailable","Diagnostic collection failed.")));
 return new(now,rows);
 }
 public static string Json(ProfessionalReport report)=>JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true});
 public static string CsvCell(string value){
 if(value.TrimStart().StartsWith('=')||value.TrimStart().StartsWith('+')||value.TrimStart().StartsWith('-')||value.TrimStart().StartsWith('@')||value.StartsWith('\t')||value.StartsWith('\r')||value.StartsWith('\n'))value="'"+value;
 return "\""+value.Replace("\"","\"\"")+"\"";
 }
 public static string Csv(ProfessionalReport report)=>"CollectedAt,Section,Item,Status,Detail\r\n"+string.Join("\r\n",report.Rows.Select(r=>string.Join(",",new[]{report.CollectedAt.ToString("O"),r.Section,r.Item,r.Status,r.Detail}.Select(CsvCell))));
 public static string Html(ProfessionalReport report){
 string E(string s)=>WebUtility.HtmlEncode(s);
 return "<!doctype html><html><head><meta charset=\"utf-8\"><title>Droid Lab report</title><style>body{font:15px system-ui;margin:40px;color:#182433}h1{color:#087d68}table{border-collapse:collapse;width:100%}td,th{border:1px solid #ccd5df;padding:9px;text-align:left;vertical-align:top}tr{break-inside:avoid}</style></head><body><h1>Droid Lab | Diagnostic report</h1><p>"+E(report.CollectedAt.ToString("O"))+"</p><p>Reported observations only; no confirmed vulnerability or hardware certification.</p><table><thead><tr><th>Section</th><th>Item</th><th>Status</th><th>Detail</th></tr></thead><tbody>"+string.Concat(report.Rows.Select(r=>"<tr><td>"+E(r.Section)+"</td><td>"+E(r.Item)+"</td><td>"+E(r.Status)+"</td><td>"+E(r.Detail)+"</td></tr>"))+"</tbody></table></body></html>";
 }
}