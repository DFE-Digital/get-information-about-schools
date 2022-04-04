using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Edubase.UnitTest.Mocks
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        public bool AlwaysTimeout { get; set; }

        public Dictionary<Uri, HttpResponseMessage> Response2UriMap { get; private set; } = new Dictionary<Uri, HttpResponseMessage>();

        public void Add(Uri uri, HttpResponseMessage responseMessage) => Response2UriMap.Add(uri, responseMessage);

        public void Clear() => Response2UriMap.Clear();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (AlwaysTimeout) throw new TaskCanceledException();

            if (Response2UriMap.ContainsKey(request.RequestUri)) return Task.FromResult(Response2UriMap[request.RequestUri]);
            else return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request });
        }
    }
}
