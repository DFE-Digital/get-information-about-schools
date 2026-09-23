using System.Net.Http.Headers;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;

namespace Edubase.AcceptanceTests.Authentication
{

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

        public static IElement GetElement(this IParentNode document, string cssSelector)
        {
            if (string.IsNullOrEmpty(cssSelector))
            {
                throw new ArgumentException("selector cannot be null or empty", nameof(cssSelector));
            }
            return document.QuerySelector(cssSelector) ?? throw new ArgumentException($"Element not found with selector {cssSelector}");
        }

        public static IEnumerable<IElement> GetMultipleElements(this IParentNode document, string cssSelector)
        {
            if (string.IsNullOrEmpty(cssSelector))
            {
                throw new ArgumentException("selector cannot be null or empty", nameof(cssSelector));
            }
            return document.QuerySelectorAll(cssSelector) ??
                    throw new ArgumentNullException($"Multiple elements not found with selector {cssSelector}");
        }

        public static string GetElementText(this IParentNode document, string cssSelector)
        {
            var elementNeeded = document.GetElement(cssSelector);
            return elementNeeded.TextContent.Trim();
        }

        public static string GetElementLinkValue(this IParentNode document, string cssSelector) => document.GetElement(cssSelector).GetAttribute("href");

        public static IEnumerable<string> GetMultipleElementText(this IParentNode document, string cssSelector)
            => document.GetMultipleElements(cssSelector).Select(t => t.TextContent.Trim());

        public static IEnumerable<string> GetMultipleChildrenElementText(this IParentNode document, string cssSelector)
            => document.GetMultipleElements(cssSelector)
                .SelectMany(t => t.Children)
                    .Select(t => t.TextContent.Trim());
    }
}
