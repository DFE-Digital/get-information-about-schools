namespace Edubase.Services
{
    using Core;
    using Data.Repositories;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using Texuna.Core;
    using Autofac.Features.AttributeFilters;

    public class ApiClientWrapper : ClientWrapperBase
    {
        public ApiClientWrapper([KeyFilter(nameof(ApiClientWrapper))] HttpClient httpClient, JsonMediaTypeFormatter formatter, IClientStorage clientStorage, ApiRecorderSessionItemRepository apiRecorderSessionItemRepository)
            : base (httpClient, formatter, clientStorage, apiRecorderSessionItemRepository)
        {
        }
    }
}
