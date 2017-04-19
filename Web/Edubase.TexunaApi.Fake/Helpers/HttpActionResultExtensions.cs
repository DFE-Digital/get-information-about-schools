using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Edubase.TexunaApi.Fake.Helpers
{
    public static class HttpActionResultExtensions
    {
        public static MockResult<T> Mockify<T>(this T actionResult) where T : IHttpActionResult
        {
            return new MockResult<T>(actionResult);
        }
    }
}