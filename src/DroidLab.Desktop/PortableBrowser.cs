using System.Runtime.InteropServices;
namespace DroidLab.Desktop;
internal sealed record PortableEntry(string Name,string Type,bool Folder,object Item);
internal sealed class PortableBrowser:IDisposable{
 readonly dynamic shell;
 readonly Stack<object> history=new();
 object? current;
 public PortableBrowser(){shell=Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application")!)!;}
 public IReadOnlyList<PortableEntry> Root(){history.Clear();current=shell.NameSpace(17);return Read(true);}
 IReadOnlyList<PortableEntry> Read(bool root=false){dynamic folder=current??throw new InvalidOperationException("Open inventory first.");var result=new List<PortableEntry>();foreach(dynamic item in folder.Items()){if(root&&(bool)item.IsFileSystem)continue;result.Add(new((string)item.Name,(string)item.Type,(bool)item.IsFolder,(object)item));}return result;}
 public IReadOnlyList<PortableEntry> Open(PortableEntry entry){if(!entry.Folder)throw new InvalidOperationException("Select a folder.");dynamic item=entry.Item;object next=item.GetFolder;history.Push(current!);current=next;return Read();}
 public IReadOnlyList<PortableEntry> Back(){if(history.Count==0)return Root();current=history.Pop();return Read(history.Count==0);}
 public void CopyTo(PortableEntry entry,string destination){if(entry.Folder)throw new InvalidOperationException("Select a single file.");if(!Directory.Exists(destination))throw new DirectoryNotFoundException();dynamic target=shell.NameSpace(destination);target.CopyHere(entry.Item,0);}
 public void Dispose(){history.Clear();current=null;if(Marshal.IsComObject(shell))Marshal.FinalReleaseComObject(shell);}
}