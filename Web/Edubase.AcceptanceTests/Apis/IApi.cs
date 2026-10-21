using AngleSharp.Html.Dom;

namespace Edubase.AcceptanceTests.Apis
{
    public interface IApi
    {
        Task<T> GetAsync<T>(string url);
        Task<IHtmlDocument> GetHtmlAsync(string url);
        Task<IHtmlDocument> PostFormAsync(string url, IEnumerable<KeyValuePair<string, string>> formData);
    }
}
