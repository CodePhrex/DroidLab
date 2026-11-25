using System.Security.Cryptography;
using System.Text.Json;
namespace DroidLab.Devices;
public enum Edition { Community, Professional, Enterprise }
public enum LicensedFeature { Basic, AdvancedDiagnostics, Recovery, Security, Ai, Technician, Reporting }
public record LicenseClaims(string LicenseId,string Product,Edition Edition,DateTimeOffset NotBefore,DateTimeOffset Expires);
public record SignedLicense(string Payload,string Signature);
public record LicenseStatus(Edition Edition,bool Valid,string Detail,DateTimeOffset? Expires);
public static class Licensing {
 public static decimal MonthlyPrice(Edition edition)=>edition switch{Edition.Community=>0m,Edition.Professional=>19.99m,Edition.Enterprise=>49.99m,_=>throw new ArgumentOutOfRangeException(nameof(edition))};
 public static bool Allows(Edition edition,LicensedFeature feature)=>Enum.IsDefined(edition)&&feature switch{LicensedFeature.Basic=>true,LicensedFeature.AdvancedDiagnostics or LicensedFeature.Recovery or LicensedFeature.Security or LicensedFeature.Ai=>edition>=Edition.Professional,LicensedFeature.Technician or LicensedFeature.Reporting=>edition==Edition.Enterprise,_=>false};
 public static LicenseStatus Validate(string envelope,string publicKeyPem,DateTimeOffset now){
 LicenseStatus Invalid(string detail)=>new(Edition.Community,false,detail,null);
 if(string.IsNullOrWhiteSpace(publicKeyPem))return Invalid("No trusted issuer key configured.");
 if(string.IsNullOrWhiteSpace(envelope)||envelope.Length>65536)return Invalid("Invalid license size.");
 try{
 var signed=JsonSerializer.Deserialize<SignedLicense>(envelope)??throw new FormatException();
 var payload=Convert.FromBase64String(signed.Payload);var signature=Convert.FromBase64String(signed.Signature);
 using var rsa=RSA.Create();rsa.ImportFromPem(publicKeyPem);
 if(rsa.KeySize<2048||!rsa.VerifyData(payload,signature,HashAlgorithmName.SHA256,RSASignaturePadding.Pss))return Invalid("Signature verification failed.");
 var claims=JsonSerializer.Deserialize<LicenseClaims>(payload)??throw new FormatException();
 if(claims.Product!="DroidLab"||string.IsNullOrWhiteSpace(claims.LicenseId)||!Enum.IsDefined(claims.Edition)||claims.Edition==Edition.Community)return Invalid("Invalid license claims.");
 if(claims.Expires<=claims.NotBefore||now<claims.NotBefore||now>=claims.Expires)return Invalid("License outside validity period.");
 return new(claims.Edition,true,"Signed license verified.",claims.Expires);
 }catch(Exception ex) when(ex is JsonException or FormatException or ArgumentException or CryptographicException or InvalidOperationException){return Invalid("Malformed or invalid license.");}
 }
}
public interface ISubscriptionProvider {
 Task<string> GetCheckoutUrlAsync(Edition edition,CancellationToken token=default);
 Task<string> GetManagementUrlAsync(CancellationToken token=default);
}