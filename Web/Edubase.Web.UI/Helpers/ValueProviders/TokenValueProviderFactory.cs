using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Common;
using Edubase.Data.Repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Edubase.Web.UI.Helpers.ValueProviders
{
    public class TokenValueProviderFactory : IValueProviderFactory
    {
        public async Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            var tokenId = context.ActionContext.HttpContext.Request.Query["tok"].ToString().Clean();
            if (!string.IsNullOrEmpty(tokenId))
            {
                var repo = context.ActionContext.HttpContext.RequestServices.GetService<ITokenRepository>();
                var token = await repo.GetAsync(tokenId); // Assuming async method in .NET 8

                if (token != null)
                {
                    var nvp = System.Web.HttpUtility.ParseQueryString(token.Data);
                    var dictionary = nvp.AllKeys.ToDictionary(k => k, k => (string) nvp[k]);
                    var valueProvider = new DictionaryValueProvider<string>(dictionary, CultureInfo.CurrentCulture);
                    context.ValueProviders.Add((Microsoft.AspNetCore.Mvc.ModelBinding.IValueProvider) valueProvider);
                }
            }
        }
    }
}
