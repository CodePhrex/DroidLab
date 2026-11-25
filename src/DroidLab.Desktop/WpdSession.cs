using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using DroidLab.Devices;
namespace DroidLab.Desktop;
internal sealed class WpdSession:IDisposable{
 [StructLayout(LayoutKind.Sequential)] internal struct Key{public Guid Format;public uint Id;public Key(string format,uint id){Format=new Guid(format);Id=id;}}
 [ComImport,Guid("625e2df8-6392-4cf0-9ad1-3cfa5f17775c"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)] interface Device{
 void Open([MarshalAs(UnmanagedType.LPWStr)] string id,[MarshalAs(UnmanagedType.Interface)] object info);
 void SendCommand(uint flags,object parameters,out object results);
 void Content(out Content content);
 void Capabilities(out object capabilities);
 void Cancel();void Close();
 }
 [ComImport,Guid("6a96ed84-7c73-4480-9938-bf5af477d426"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)] interface Content{
 void EnumObjects(uint flags,[MarshalAs(UnmanagedType.LPWStr)] string parent,[MarshalAs(UnmanagedType.Interface)] object? filter,out ObjectIds ids);
 void Properties(out object properties);void Transfer(out Resources resources);
 }
 [ComImport,Guid("10ece955-cf41-4728-bfa0-41eedf1bbf19"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)] interface ObjectIds{
 [PreserveSig] int Next(uint count,[MarshalAs(UnmanagedType.LPArray,ArraySubType=UnmanagedType.LPWStr,SizeParamIndex=0),Out] string[] ids,ref uint fetched);
 }
 [ComImport,Guid("fd8878ac-d841-4d17-891c-e6829cdb6934"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)] interface Resources{
 void GetSupportedResources([MarshalAs(UnmanagedType.LPWStr)] string id,out object keys);
 void GetResourceAttributes([MarshalAs(UnmanagedType.LPWStr)] string id,ref Key key,out object attributes);
 void GetStream([MarshalAs(UnmanagedType.LPWStr)] string id,ref Key key,uint mode,ref uint optimal,out IStream stream);
 }
 readonly Device device;readonly Content content;bool disposed;
 static object Create(string clsid)=>Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid(clsid),true)!)!;
 static void Release(object o){if(Marshal.IsComObject(o))Marshal.ReleaseComObject(o);}
 public WpdSession(string id){device=(Device)Create("728a21c5-3d9e-48d7-9810-864848f0f404");var info=Create("0c15d503-d017-47ce-9016-7b3f978721cc");try{device.Open(id,info);device.Content(out content);}catch{Release(device);throw;}finally{Release(info);}}
 public IReadOnlyList<string> Children(string parent){ObjectDisposedException.ThrowIf(disposed,this);content.EnumObjects(0,parent,null,out var ids);try{var result=new List<string>();while(true){var batch=new string[16];uint fetched=0;int hr=ids.Next(16,batch,ref fetched);Marshal.ThrowExceptionForHR(hr);for(int i=0;i<fetched;i++)result.Add(batch[i]);if(result.Count>100000)throw new IOException("Object enumeration limit exceeded.");if(hr!=0||fetched==0)break;}return result;}finally{Release(ids);}}
 public async Task<TransferReceipt> Download(string id,string destination,long expected,IProgress<long>? progress,CancellationToken token){ObjectDisposedException.ThrowIf(disposed,this);content.Transfer(out var resources);try{var key=new Key("e81e79be-34f0-41bf-b53f-f1a06ae87842",0);uint optimal=0;resources.GetStream(id,ref key,0,ref optimal,out var stream);try{using var source=new ComReadStream(stream);return await VerifiedTransfer.CopyAsync(source,destination,expected,progress,token);}finally{Release(stream);}}finally{Release(resources);}}
 public void Dispose(){if(disposed)return;disposed=true;try{device.Close();}finally{Release(content);Release(device);}}
 sealed class ComReadStream(IStream stream):Stream{
 public override bool CanRead=>true;public override bool CanSeek=>false;public override bool CanWrite=>false;public override long Length=>throw new NotSupportedException();public override long Position{get=>throw new NotSupportedException();set=>throw new NotSupportedException();}
 public override int Read(byte[] buffer,int offset,int count){var data=new byte[count];var ptr=Marshal.AllocCoTaskMem(4);try{Marshal.WriteInt32(ptr,0);stream.Read(data,count,ptr);var n=Marshal.ReadInt32(ptr);if(n<0||n>count)throw new IOException("Invalid COM read count.");Array.Copy(data,0,buffer,offset,n);return n;}finally{Marshal.FreeCoTaskMem(ptr);}}
 public override ValueTask<int> ReadAsync(Memory<byte> buffer,CancellationToken cancellationToken=default){cancellationToken.ThrowIfCancellationRequested();var data=new byte[buffer.Length];int n=Read(data,0,data.Length);data.AsMemory(0,n).CopyTo(buffer);return ValueTask.FromResult(n);}
 public override void Flush()=>throw new NotSupportedException();public override long Seek(long offset,SeekOrigin origin)=>throw new NotSupportedException();public override void SetLength(long value)=>throw new NotSupportedException();public override void Write(byte[] buffer,int offset,int count)=>throw new NotSupportedException();
 }
}