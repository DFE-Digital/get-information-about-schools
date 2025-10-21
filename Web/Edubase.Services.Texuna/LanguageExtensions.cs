using Edubase.Common;
using Edubase.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Texuna;

public static class LanguageExtensions
{
    public static string GetUserId(this IPrincipal principal) => (principal?.Identity as ClaimsIdentity)?.FindFirst(EduClaimTypes.UserId)?.Value?.Clean();

    public static string UrlTokenize(this object obj, string name)
    {
        var str = obj?.ToString()?.Clean();
        if (str != null) return $"{name}={str}&";
        else return string.Empty;
    }
}
