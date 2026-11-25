using DroidLab.Devices;
using System.Security.Cryptography;
using System.Text.Json;
using Xunit;
public class LicensingTests {
 static DateTimeOffset Now=>DateTimeOffset.UnixEpoch.AddDays(10);
 static string Sign(RSA key,LicenseClaims claims){var data=JsonSerializer.SerializeToUtf8Bytes(claims);return JsonSerializer.Serialize(new SignedLicense(Convert.ToBase64String(data),Convert.ToBase64String(key.SignData(data,HashAlgorithmName.SHA256,RSASignaturePadding.Pss))));}
 static LicenseClaims Claims=>new("test","DroidLab",Edition.Professional,Now.AddDays(-1),Now.AddDays(1));
 [Fact] public void CommunityBasic()=>Assert.True(Licensing.Allows(Edition.Community,LicensedFeature.Basic));
 [Fact] public void CommunityNoAi()=>Assert.False(Licensing.Allows(Edition.Community,LicensedFeature.Ai));
 [Fact] public void ProfessionalNoTechnician()=>Assert.False(Licensing.Allows(Edition.Professional,LicensedFeature.Technician));
 [Fact] public void EnterpriseReporting()=>Assert.True(Licensing.Allows(Edition.Enterprise,LicensedFeature.Reporting));
 [Fact] public void ApprovedPrices()=>Assert.Equal(19.99m,Licensing.MonthlyPrice(Edition.Professional));
 [Fact] public void ValidSignature(){using var key=RSA.Create(2048);Assert.True(Licensing.Validate(Sign(key,Claims),key.ExportSubjectPublicKeyInfoPem(),Now).Valid);}
 [Fact] public void WrongKeyRejected(){using var key=RSA.Create(2048);using var other=RSA.Create(2048);Assert.False(Licensing.Validate(Sign(key,Claims),other.ExportSubjectPublicKeyInfoPem(),Now).Valid);}
 [Fact] public void ExpiredRejected(){using var key=RSA.Create(2048);Assert.False(Licensing.Validate(Sign(key,Claims),key.ExportSubjectPublicKeyInfoPem(),Now.AddDays(2)).Valid);}
 [Fact] public void WrongProductRejected(){using var key=RSA.Create(2048);Assert.False(Licensing.Validate(Sign(key,Claims with{Product="Other"}),key.ExportSubjectPublicKeyInfoPem(),Now).Valid);}
 [Fact] public void MissingIssuerRejected()=>Assert.False(Licensing.Validate("{}","",Now).Valid);
}