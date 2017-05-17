using System.Net.Http;
using System.Net.Http.Headers;

namespace Edubase.TexunaApi.Fake.Helpers
{
    public class MockMessage
    {
        public MediaTypeHeaderValue ContentType { get; set; }
        public string Content { get; set; }

        public MockContent ToHttpContent() => new MockContent(this.ContentType, this.Content);
    }
}