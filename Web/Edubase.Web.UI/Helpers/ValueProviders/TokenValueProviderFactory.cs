using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

/// <summary>
/// A custom <see cref="IValueProviderFactory"/> that retrieves values from a token stored in the query string.
/// </summary>
/// <remarks>
/// This factory looks for a "tok" parameter in the query string, fetches the token from the repository,
/// and exposes its data as query string values for model binding.
/// </remarks>
public class TokenValueProviderFactory(
    ITokenRepository tokenRepository) : IValueProviderFactory
{
    /// <summary>
    /// Creates a value provider that supplies values from a token if present in the query string.
    /// </summary>
    /// <param name="context">The <see cref="ValueProviderFactoryContext"/> containing the current request and value providers.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CreateValueProviderAsync(ValueProviderFactoryContext context)
    {
        const string TokenQueryStringParam = "tok";

        HttpRequest request =
            context.ActionContext.HttpContext.Request;

        string tokenId =
            request.Query[TokenQueryStringParam].ToString();

        if (!string.IsNullOrEmpty(tokenId))
        {
            Token token = await tokenRepository.GetAsync(tokenId);

            if (token != null)
            {
                Dictionary<string, StringValues> dict =
                    QueryHelpers.ParseQuery(token.Data);

                QueryCollection queryCollection = new(dict);

                QueryStringValueProvider valueProvider =
                    new(
                        BindingSource.Query,
                        queryCollection,
                        CultureInfo.CurrentCulture);

                context.ValueProviders.Add(valueProvider);
            }
        }
    }
}
