using DroidLab.Devices;
using Xunit;
public class ServiceHistoryTests {
 [Fact] public async Task PersistsAndSearches(){var dir=Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString("N"));try{var h=new ServiceHistory(dir);var record=await h.AddAsync("serial","Diagnostic","Battery check");var loaded=await new ServiceHistory(dir).SearchAsync("battery");Assert.Equal(record,Assert.Single(loaded));Assert.Empty(await h.SearchAsync("missing"));}finally{if(Directory.Exists(dir))Directory.Delete(dir,true);}}
 [Fact] public async Task RejectsBlankSerial(){var h=new ServiceHistory(Path.GetTempPath());await Assert.ThrowsAsync<ArgumentException>(()=>h.AddAsync("","Diagnostic",""));}
 [Fact] public void ExportIncludesRecord(){Assert.Contains("Battery",ServiceHistory.Export(new[]{new ServiceRecord(Guid.NewGuid(),DateTimeOffset.UtcNow,"serial","Battery","")}));}
}