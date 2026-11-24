using Xunit;
using DroidLab.Devices;
public class FirmwareTests {
 [Fact] public async Task HashAndCompare(){var path=Path.GetTempFileName();try{await File.WriteAllTextAsync(path,"abc");var result=await FirmwareInspector.InspectAsync(path,"ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad");Assert.True(result.MatchesExpected==true);Assert.Equal(3L,result.Bytes);Assert.False((await FirmwareInspector.InspectAsync(path,new string('0',64))).MatchesExpected==true);}finally{File.Delete(path);}}
 [Fact] public async Task InvalidHash()=>await Assert.ThrowsAsync<ArgumentException>(()=>FirmwareInspector.InspectAsync("unused","bad"));
}