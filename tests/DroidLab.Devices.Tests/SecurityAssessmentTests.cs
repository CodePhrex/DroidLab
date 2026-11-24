using Xunit;
using DroidLab.Devices;
public class SecurityAssessmentTests {
 [Fact] public void ParsesIdentity(){var i=IdentityParser.Parse("[ro.product.model]: [Pixel]\n[ro.build.version.security_patch]: [2026-01-05]");Assert.Equal("Pixel",i.Model);Assert.Equal("2026-01-05",i.SecurityPatch);}
 [Fact] public void OldPatchIsReviewNotConfirmed(){var i=IdentityParser.Parse("[ro.build.version.security_patch]: [2025-01-05]");var f=SecurityAssessment.Assess(i,new DateOnly(2026,1,1));Assert.Equal("Review",f[0].Status);Assert.Equal("Not assessed",f[2].Status);}
 [Fact] public void MissingPatchIsUnknown(){Assert.Equal("Unknown",SecurityAssessment.Assess(IdentityParser.Parse(""),new DateOnly(2026,1,1))[0].Status);}
 [Fact] public void FuturePatchIsUnknown(){Assert.Equal("Unknown",SecurityAssessment.Assess(IdentityParser.Parse("[ro.build.version.security_patch]: [2027-01-01]"),new DateOnly(2026,1,1))[0].Status);}
}