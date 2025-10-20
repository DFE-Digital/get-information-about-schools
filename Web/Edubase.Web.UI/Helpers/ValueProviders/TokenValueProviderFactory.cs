using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Common;
using Edubase.Data.Repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Edubase.Web.UI.Helpers.ValueProviders
{
    public class TokenValueProviderFactory : IValueProviderFactory
    {
        public async Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            var tokenId = context.ActionContext.HttpContext.Request.Query["tok"].ToString();
            if (!string.IsNullOrWhiteSpace(tokenId.Clean()))
            {
                var repo = context.ActionContext.HttpContext.RequestServices.GetService<ITokenRepository>();
                var token = await repo.GetAsync(tokenId); // assuming async version; use `.Get(tokenId)` if sync

                if (token != null && !string.IsNullOrWhiteSpace(token.Data))
                {
                    var parsed = QueryHelpers.ParseQuery(token.Data);
                    var dictionary = parsed.ToDictionary(
                        kvp => kvp.Key,
                        kvp => new Microsoft.Extensions.Primitives.StringValues(kvp.Value.ToArray())
                    );

                    context.ValueProviders.Add(new QueryStringValueProvider(
                        BindingSource.Query,
                        new Microsoft.AspNetCore.Http.QueryCollection(dictionary),
                        CultureInfo.CurrentCulture));
                }
            }
        }
    }
}
