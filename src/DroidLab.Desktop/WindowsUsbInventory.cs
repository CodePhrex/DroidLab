using System.Runtime.InteropServices;
using System.Text;
using DroidLab.Devices;
namespace DroidLab.Desktop;
internal record UsbHardware(string Name,string Manufacturer,string Vendor,string Product,string Driver,string Status);
internal static class WindowsUsbInventory{
 [StructLayout(LayoutKind.Sequential)] struct Info{public uint Size;public Guid ClassGuid;public uint DevInst;public IntPtr Reserved;}
 [DllImport("setupapi.dll",CharSet=CharSet.Unicode,SetLastError=true)] static extern IntPtr SetupDiGetClassDevsW(IntPtr guid,string enumerator,IntPtr parent,uint flags);
 [DllImport("setupapi.dll",SetLastError=true)] static extern bool SetupDiEnumDeviceInfo(IntPtr set,uint index,ref Info info);
 [DllImport("setupapi.dll",CharSet=CharSet.Unicode,SetLastError=true)] static extern bool SetupDiGetDeviceInstanceIdW(IntPtr set,ref Info info,StringBuilder id,uint size,out uint required);
 [DllImport("setupapi.dll",CharSet=CharSet.Unicode,SetLastError=true)] static extern bool SetupDiGetDeviceRegistryPropertyW(IntPtr set,ref Info info,uint property,out uint type,byte[] buffer,uint size,out uint required);
 [DllImport("setupapi.dll")] static extern bool SetupDiDestroyDeviceInfoList(IntPtr set);
 [DllImport("cfgmgr32.dll")] static extern uint CM_Get_DevNode_Status(out uint status,out uint problem,uint devInst,uint flags);
 public static IReadOnlyList<UsbHardware> Scan(){
 var set=SetupDiGetClassDevsW(IntPtr.Zero,"USB",IntPtr.Zero,6);if(set==new IntPtr(-1))throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
 try{var rows=new List<UsbHardware>();for(uint i=0;;i++){var info=new Info{Size=(uint)Marshal.SizeOf<Info>()};if(!SetupDiEnumDeviceInfo(set,i,ref info)){var error=Marshal.GetLastWin32Error();if(error==259)break;throw new System.ComponentModel.Win32Exception(error);}
 var id=new StringBuilder(4096);if(!SetupDiGetDeviceInstanceIdW(set,ref info,id,4096,out _))continue;var usb=UsbIdentity.Parse(id.ToString());if(usb==null)continue;
 string Prop(uint key){var buffer=new byte[8192];return SetupDiGetDeviceRegistryPropertyW(set,ref info,key,out var type,buffer,(uint)buffer.Length,out var size)&&type==1&&size<=buffer.Length?Encoding.Unicode.GetString(buffer,0,(int)size).TrimEnd('\0'):"";}
 var name=Prop(12);if(name.Length==0)name=Prop(0);var manufacturer=Prop(11);var driver=Prop(9);var code=CM_Get_DevNode_Status(out var status,out var problem,info.DevInst,0);var state=code!=0?"Status unavailable":(status&0x400)!=0?$"Windows problem {problem}":(status&8)!=0?"Started":"Not started";
 rows.Add(new(name.Length==0?"USB device":name,manufacturer.Length==0?"Not reported":manufacturer,usb.VendorId+" · "+usb.VendorHint,usb.ProductId,driver.Length==0?"Driver key not reported":"Driver key present",state));}return rows;}finally{SetupDiDestroyDeviceInfoList(set);}
 }
}