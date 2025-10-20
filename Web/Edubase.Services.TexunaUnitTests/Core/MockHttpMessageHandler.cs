using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Edubase.Services.TexunaUnitTests.Core;

public class MockHttpMessageHandler : HttpMessageHandler
{
    public bool AlwaysTimeout { get; set; }

    private Dictionary<Uri, HttpResponseMessage> Response2UriMap { get; }
        = new Dictionary<Uri, HttpResponseMessage>();

    public void Add(Uri uri, HttpResponseMessage responseMessage)
    {
        Response2UriMap.Add(uri, responseMessage);
    }

    public void Clear()
    {
        Response2UriMap.Clear();
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (AlwaysTimeout)
        {
            throw new TaskCanceledException();
        }

        return Task.FromResult(Response2UriMap.ContainsKey(request.RequestUri)
            ? Response2UriMap[request.RequestUri]
            : new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request });
    }
}
