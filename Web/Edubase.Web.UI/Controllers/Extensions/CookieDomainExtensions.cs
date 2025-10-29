using Microsoft.AspNetCore.Http;

namespace Edubase.Web.UI.Controllers.Extensions;

public static class CookieDomainExtensions
{
    public static string GetCookieDomain(this HttpRequest request)
    {
        var host = request.Host.Host;

        // Strip subdomain (e.g., "admin.example.com" → ".example.com")
        var parts = host.Split('.');
        if (parts.Length >= 2)
        {
            return $".{parts[^2]}.{parts[^1]}";
        }

        return host; // fallback for localhost or IPs
    }
}

