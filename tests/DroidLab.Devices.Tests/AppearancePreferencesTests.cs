using DroidLab.Devices;
using Xunit;
public class AppearancePreferencesTests{
 [Theory][InlineData("Light",AppearanceMode.Light)][InlineData("Dark",AppearanceMode.Dark)][InlineData("invalid",AppearanceMode.Dark)][InlineData(null,AppearanceMode.Dark)] public void Parse(string? raw,AppearanceMode expected)=>Assert.Equal(expected,AppearancePreferences.Parse(raw));
 [Fact] public void RoundTrip(){var root=Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString("N"));var path=Path.Combine(root,"theme.txt");try{Assert.Equal(AppearanceMode.Dark,AppearancePreferences.Load(path));AppearancePreferences.Save(path,AppearanceMode.Light);Assert.Equal(AppearanceMode.Light,AppearancePreferences.Load(path));AppearancePreferences.Save(path,AppearanceMode.Dark);Assert.Equal(AppearanceMode.Dark,AppearancePreferences.Load(path));}finally{if(Directory.Exists(root))Directory.Delete(root,true);}}
 [Fact] public void OversizedFallsBack(){var path=Path.GetTempFileName();try{File.WriteAllText(path,new string('x',65));Assert.Equal(AppearanceMode.Dark,AppearancePreferences.Load(path));}finally{File.Delete(path);}}
}