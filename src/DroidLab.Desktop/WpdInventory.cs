using System.Runtime.InteropServices;
namespace DroidLab.Desktop;
internal sealed record WpdDevice(string Id,string Name);
internal static class WpdInventory{
 [ComImport,Guid("A1567595-4C2F-4574-A6FA-ECEF917B9A40"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
 interface IPortableDeviceManager{
 void GetDevices(IntPtr ids,ref uint count);
 void RefreshDeviceList();
 void GetDeviceFriendlyName([MarshalAs(UnmanagedType.LPWStr)] string id,IntPtr name,ref uint count);
 }
 public static IReadOnlyList<WpdDevice> Enumerate(){
 var type=Type.GetTypeFromCLSID(new Guid("0AF10CEC-2ECD-4B92-9581-34F6AE0637F3"),true)!;var instance=Activator.CreateInstance(type)!;
 try{var manager=(IPortableDeviceManager)instance;manager.RefreshDeviceList();uint count=0;manager.GetDevices(IntPtr.Zero,ref count);if(count==0)return Array.Empty<WpdDevice>();if(count>4096)throw new InvalidOperationException("Unexpected WPD inventory size.");int capacity=(int)count;var buffer=Marshal.AllocCoTaskMem(capacity*IntPtr.Size);try{for(int i=0;i<capacity;i++)Marshal.WriteIntPtr(buffer,i*IntPtr.Size,IntPtr.Zero);manager.GetDevices(buffer,ref count);var result=new List<WpdDevice>();for(int i=0;i<Math.Min(capacity,(int)count);i++){var id=Marshal.PtrToStringUni(Marshal.ReadIntPtr(buffer,i*IntPtr.Size));if(!string.IsNullOrEmpty(id))result.Add(new(id,FriendlyName(manager,id)));}return result;}finally{for(int i=0;i<capacity;i++){var ptr=Marshal.ReadIntPtr(buffer,i*IntPtr.Size);if(ptr!=IntPtr.Zero)Marshal.FreeCoTaskMem(ptr);}Marshal.FreeCoTaskMem(buffer);}}
 finally{Marshal.FinalReleaseComObject(instance);}
 }
 static string FriendlyName(IPortableDeviceManager manager,string id){try{uint length=0;manager.GetDeviceFriendlyName(id,IntPtr.Zero,ref length);if(length==0||length>32768)return "Unnamed portable device";var buffer=Marshal.AllocCoTaskMem(checked((int)length*2));try{manager.GetDeviceFriendlyName(id,buffer,ref length);return Marshal.PtrToStringUni(buffer)??"Unnamed portable device";}finally{Marshal.FreeCoTaskMem(buffer);}}catch(COMException){return "Unnamed portable device";}}
 public static int Count()=>Enumerate().Count;
}