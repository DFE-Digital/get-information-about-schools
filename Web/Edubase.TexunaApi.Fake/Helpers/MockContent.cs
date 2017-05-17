using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Edubase.TexunaApi.Fake.Helpers
{
    public class MockContent : HttpContent
    {
        private readonly string content;

        public MockContent(MediaTypeHeaderValue contentType, string content)
        {
            this.content = content;
            Id = Guid.NewGuid();
            Headers.ContentType = contentType;
            Headers.Add("x-mock-response-id", Id.ToString("N"));
            
        }

        public Guid Id { get; }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(content);
                writer.Flush();
            }

            return Task.CompletedTask;
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
}