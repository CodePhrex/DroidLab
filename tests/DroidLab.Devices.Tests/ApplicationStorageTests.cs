using DroidLab.Devices;
using Xunit;
public class ApplicationStorageTests {
 [Fact] public void FiltersPackages()=>Assert.Equal(new[]{"com.example.app"},ApplicationStorage.Packages("package:com.example.app\npackage:org.other.app\npackage:com.example.app","example"));
 [Fact] public void RejectsShellInjection()=>Assert.Throws<ArgumentException>(()=>ApplicationStorage.PackageArguments("com.app;id"));
 [Fact] public void MetadataArguments()=>Assert.Equal(new[]{"shell","dumpsys","package","com.example.app"},ApplicationStorage.PackageArguments("com.example.app"));
 [Fact] public void UninstallArguments()=>Assert.Equal(new[]{"uninstall","com.example.app"},ApplicationStorage.PackageArguments("com.example.app",true));
 [Theory][InlineData("../data")][InlineData("Android/data")][InlineData("/data")][InlineData("Download/a;id")] public void RejectsUnsafePaths(string path)=>Assert.Throws<ArgumentException>(()=>ApplicationStorage.SharedPath(path));
 [Fact] public void SharedFolder()=>Assert.Equal("/sdcard/Download",ApplicationStorage.SharedPath("Download"));
 [Fact] public void RejectsNonApk()=>Assert.Throws<ArgumentException>(()=>ApplicationStorage.InstallArguments("file.txt"));
}