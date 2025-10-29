using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Edubase.Web.UI.Helpers;

public static class HtmlPrefixScopeExtensions
{
    public static IDisposable BeginCollectionItem(this IHtmlHelper html, string collectionName)
    {
        var itemIndex = Guid.NewGuid().ToString();
        var htmlFieldPrefix = $"{collectionName}[{itemIndex}]";

        var originalPrefix = html.ViewData.TemplateInfo.HtmlFieldPrefix;
        html.ViewData.TemplateInfo.HtmlFieldPrefix = htmlFieldPrefix;

        return new CollectionItemScope(html.ViewData, originalPrefix);
    }

    private class CollectionItemScope : IDisposable
    {
        private readonly ViewDataDictionary _viewData;
        private readonly string _originalPrefix;

        public CollectionItemScope(ViewDataDictionary viewData, string originalPrefix)
        {
            _viewData = viewData;
            _originalPrefix = originalPrefix;
        }

        public void Dispose()
        {
            _viewData.TemplateInfo.HtmlFieldPrefix = _originalPrefix;
        }
    }
}

