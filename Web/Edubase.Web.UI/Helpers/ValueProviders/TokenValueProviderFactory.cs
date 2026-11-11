using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Common;
using Edubase.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Edubase.Web.UI.Helpers.ValueProviders
{
    public class TokenValueProviderFactory : IValueProviderFactory
    {
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            var tokenId = context.ActionContext.HttpContext.Request.Query["tok"].ToString();
            if (tokenId.Clean() == null)
            {
                return Task.CompletedTask;
            }

            var repo = context.ActionContext.HttpContext.RequestServices.GetService<ITokenRepository>();
            if (repo == null)
            {
                return Task.CompletedTask;
            }

            var token = repo.Get(tokenId);
            if (token == null || token.Data.Clean() == null)
            {
                return Task.CompletedTask;
            }

            var rawTokenData = token.Data.StartsWith("?") ? token.Data : "?" + token.Data;
            var parsedQuery = QueryHelpers.ParseQuery(rawTokenData)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var tokenQueryCollection = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(parsedQuery));
            var tokenValueProvider = new QueryStringValueProvider(BindingSource.Query, tokenQueryCollection, CultureInfo.CurrentCulture);

            context.ValueProviders.Add(tokenValueProvider);
            return Task.CompletedTask;
        }
    }
}
