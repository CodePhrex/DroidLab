using Xunit;
using DroidLab.Devices;
public class AdvisoryCacheTests {
 [Fact] public async Task RoundTrip(){var dir=Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString("N"));try{var cache=new AdvisoryCache(dir);var snapshot=new AdvisorySnapshot("Android",DateTimeOffset.UtcNow,new[]{new Advisory("CVE-2025-1234","Example",8.0,true)});await cache.SaveAsync(snapshot);var read=await cache.ReadAsync("android");Assert.NotNull(read);Assert.Equal(snapshot.RetrievedAt,read.RetrievedAt);Assert.True(Assert.Single(read.Advisories).KnownExploited);Assert.Null(await cache.ReadAsync("other"));}finally{if(Directory.Exists(dir))Directory.Delete(dir,true);}}
 [Fact] public void ReportDisclaimsDeviceConfirmation(){Assert.Contains("not confirmed device vulnerabilities",AdvisoryCache.Report(new("Android",DateTimeOffset.UtcNow,Array.Empty<Advisory>())));}
}