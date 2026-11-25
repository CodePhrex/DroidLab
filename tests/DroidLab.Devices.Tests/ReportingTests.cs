using DroidLab.Devices;
using Xunit;
public class ReportingTests {
 static ProfessionalReport Sample=>new(DateTimeOffset.UnixEpoch,new[]{new ReportRow("Test","Item","Unknown","<script>alert(1)</script>")});
 [Fact] public void HtmlEscapes()=>Assert.Contains("&lt;script&gt;",Reporting.Html(Sample));
 [Fact] public void NoScriptTag()=>Assert.DoesNotContain("<script>",Reporting.Html(Sample));
 [Fact] public void CsvFormulaNeutralized()=>Assert.Equal("\"'=1+1\"",Reporting.CsvCell("=1+1"));
 [Fact] public void CsvQuotes()=>Assert.Equal("\"a\"\"b\"",Reporting.CsvCell("a\"b"));
 [Fact] public void JsonIncludesRows()=>Assert.Contains("Unknown",Reporting.Json(Sample));
 [Fact] public void MissingCollectionReported(){var r=Reporting.Build(IdentityParser.Parse(""),"","","",new[]{"Battery"},DateTimeOffset.UnixEpoch);Assert.Contains(r.Rows,x=>x.Section=="Collection"&&x.Item=="Battery");}
 [Fact] public void MissingIdentityUnknown(){var r=Reporting.Build(IdentityParser.Parse(""),"","","",Array.Empty<string>(),DateTimeOffset.UnixEpoch);Assert.Contains(r.Rows,x=>x.Item=="Model"&&x.Status=="Unknown");}
}