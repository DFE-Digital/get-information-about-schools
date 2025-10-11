using Edubase.Common;
using Edubase.Data.Repositories;
using System.Globalization;
using System.Web;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Helpers.ValueProviders
{
    public class TokenValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            var tokenId = controllerContext.RequestContext.HttpContext.Request.QueryString["tok"];
            if (tokenId.Clean() != null)
            {
                var repo = DependencyResolver.Current.GetService<ITokenRepository>();
                var token = repo.Get(tokenId);
                if (token != null)
                {
                    var nvp = HttpUtility.ParseQueryString(token.Data);
                    return new NameValueCollectionValueProvider(nvp, CultureInfo.CurrentCulture);
                }
            }

            return null;
        }
    }
}