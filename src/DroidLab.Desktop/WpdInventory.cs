using System.Runtime.InteropServices;
namespace DroidLab.Desktop;
internal static class WpdInventory{
 [ComImport,Guid("A1567595-4C2F-4574-A6FA-ECEF917B9A40"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
 interface IPortableDeviceManager{
 void GetDevices([In,Out,MarshalAs(UnmanagedType.LPArray,ArraySubType=UnmanagedType.LPWStr,SizeParamIndex=1)] string[]? ids,ref uint count);
 void RefreshDeviceList();
 }
 public static int Count(){var type=Type.GetTypeFromCLSID(new Guid("0AF10CEC-2ECD-4B92-9581-34F6AE0637F3"),true)!;var instance=Activator.CreateInstance(type)!;try{var manager=(IPortableDeviceManager)instance;manager.RefreshDeviceList();uint count=0;manager.GetDevices(null,ref count);return checked((int)count);}finally{Marshal.FinalReleaseComObject(instance);}}
}