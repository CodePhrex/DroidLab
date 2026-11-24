using System.Text.Json;
namespace DroidLab.Devices;
public record ServiceRecord(Guid Id,DateTimeOffset Created,string DeviceSerial,string Operation,string Notes);
public sealed class ServiceHistory(string directory){
 readonly SemaphoreSlim gate=new(1,1);
 string PathName=>Path.Combine(directory,"service-history.json");
 async Task<List<ServiceRecord>> Read(CancellationToken token)=>File.Exists(PathName)?JsonSerializer.Deserialize<List<ServiceRecord>>(await File.ReadAllTextAsync(PathName,token))??throw new InvalidDataException("Invalid service history"):new();
 public async Task<ServiceRecord> AddAsync(string serial,string operation,string notes,CancellationToken token=default){
 if(string.IsNullOrWhiteSpace(serial)||string.IsNullOrWhiteSpace(operation)||serial.Length>256||operation.Length>128||notes.Length>4096)throw new ArgumentException("Invalid service record.");
 await gate.WaitAsync(token);try{var records=await Read(token);var record=new ServiceRecord(Guid.NewGuid(),DateTimeOffset.UtcNow,serial.Trim(),operation.Trim(),notes.Trim());records.Add(record);Directory.CreateDirectory(directory);var temp=PathName+"."+Guid.NewGuid().ToString("N")+".tmp";try{await File.WriteAllTextAsync(temp,JsonSerializer.Serialize(records),token);File.Move(temp,PathName,true);}finally{if(File.Exists(temp))File.Delete(temp);}return record;}finally{gate.Release();}
 }
 public async Task<IReadOnlyList<ServiceRecord>> SearchAsync(string query="",CancellationToken token=default){
 await gate.WaitAsync(token);try{return (await Read(token)).Where(r=>r.DeviceSerial.Contains(query,StringComparison.OrdinalIgnoreCase)||r.Operation.Contains(query,StringComparison.OrdinalIgnoreCase)||r.Notes.Contains(query,StringComparison.OrdinalIgnoreCase)).OrderByDescending(r=>r.Created).ToList();}finally{gate.Release();}
 }
 public static string Export(IReadOnlyList<ServiceRecord> records)=>JsonSerializer.Serialize(records,new JsonSerializerOptions{WriteIndented=true});
}