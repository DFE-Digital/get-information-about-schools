using Edubase.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Helpers.ValueProviders
{
    public class TokenValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            var queryString = controllerContext.RequestContext.HttpContext.Request.QueryString.ToString();
            if (queryString.Length > 4 && queryString.Length < 12 && !queryString.Contains("="))
            {
                var repo = DependencyResolver.Current.GetService<ITokenRepository>();
                var token = repo.Get(queryString);
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