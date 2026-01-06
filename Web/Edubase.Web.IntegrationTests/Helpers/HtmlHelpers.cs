using System.Net.Http.Headers;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Io;

namespace Edubase.Web.IntegrationTests.Helpers;

public static class HtmlHelpers
{
    public static async Task<IHtmlDocument> GetDocumentAsync(this HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var config = Configuration.Default.WithDefaultLoader();
        var document = await
            BrowsingContext.New(config)
                    .OpenAsync(ResponseFactory, CancellationToken.None);

        return (IHtmlDocument) document;
        void ResponseFactory(VirtualResponse htmlResponse)
        {
            htmlResponse
                .Address(response.RequestMessage!.RequestUri)
                .Status(response.StatusCode);

            MapHeaders(response.Headers);
            MapHeaders(response.Content.Headers);
            htmlResponse.Content(content);
            void MapHeaders(HttpHeaders headers)
            {
                foreach (var header in headers)
                {
                    foreach (var value in header.Value)
                    {
                        htmlResponse.Header(header.Key, value);
                    }
                }
            }
        }
    }
}
