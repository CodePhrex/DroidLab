namespace DroidLab.Devices;
public static class RecoveryReadiness {
 public static string Describe(Device device)=> (device.Transport,device.State) switch {
 ("ADB","device")=>"Online authorized ADB. Back up accessible files before recovery.",
 ("ADB","recovery")=>"Recovery detected. Available operations depend on OEM recovery.",
 ("ADB","sideload")=>"Sideload detected. Only OEM-approved compatible packages should be considered.",
 ("Fastboot","fastboot")=>"Fastboot detected. Bootloader state and image compatibility remain unverified.",
 _=>"Device not ready for recovery operations."
 };
}