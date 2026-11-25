using DroidLab.Devices;
using Xunit;
public class DroidAiTests {
 [Fact] public void LoopbackOnly()=>Assert.Equal("127.0.0.1",DroidAi.Endpoint.Host);
 [Fact] public void ModelsParsed()=>Assert.Equal(new[]{"a","b"},DroidAi.ParseModels("{\"models\":[{\"name\":\"b\"},{\"name\":\"a\"},{\"name\":\"a\"}]}"));
 [Fact] public void EmptyModels()=>Assert.Empty(DroidAi.ParseModels("{\"models\":[]}"));
 [Fact] public void BlankQuestionRejected()=>Assert.Throws<ArgumentException>(()=>DroidAi.Prompt(" ",""));
 [Fact] public void OversizeContextRejected()=>Assert.Throws<ArgumentException>(()=>DroidAi.Prompt("Explain",new string('x',24001)));
 [Fact] public void EvidenceLabeled()=>Assert.Contains("Untrusted diagnostic evidence",DroidAi.Prompt("Explain","log"));
 [Fact] public void OversizeQuestionRejected()=>Assert.Throws<ArgumentException>(()=>DroidAi.Prompt(new string('x',4001),""));
}