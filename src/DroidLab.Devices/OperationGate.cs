namespace DroidLab.Devices;
public sealed class OperationGate {
 int busy;
 public IDisposable? TryEnter()=>Interlocked.CompareExchange(ref busy,1,0)==0?new Lease(this):null;
 sealed class Lease(OperationGate owner):IDisposable {
 OperationGate? gate=owner;
 public void Dispose(){var current=Interlocked.Exchange(ref gate,null);if(current!=null)Interlocked.Exchange(ref current.busy,0);}
 }
}