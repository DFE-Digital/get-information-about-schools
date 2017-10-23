using Edubase.Common;
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