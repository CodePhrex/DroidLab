namespace DroidLab.Devices;
public enum AppearanceMode { Dark, Light }
public static class AppearancePreferences {
 public static AppearanceMode Parse(string? value)=>value?.Trim() switch {"Light"=>AppearanceMode.Light,_=>AppearanceMode.Dark};
 public static AppearanceMode Load(string path){try{return File.Exists(path)&&new FileInfo(path).Length<=64?Parse(File.ReadAllText(path)):AppearanceMode.Dark;}catch(IOException){return AppearanceMode.Dark;}catch(UnauthorizedAccessException){return AppearanceMode.Dark;}}
 public static void Save(string path,AppearanceMode mode){if(!Enum.IsDefined(mode))throw new ArgumentOutOfRangeException(nameof(mode));var full=Path.GetFullPath(path);Directory.CreateDirectory(Path.GetDirectoryName(full)!);var temp=full+"."+Guid.NewGuid().ToString("N")+".tmp";try{File.WriteAllText(temp,mode.ToString());File.Move(temp,full,true);}finally{if(File.Exists(temp))File.Delete(temp);}}
}