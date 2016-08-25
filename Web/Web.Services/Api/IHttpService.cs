using Web.Services.Api.Models;

namespace Web.Services.Api
{
    public interface IHttpService
    {
        dynamic ExecuteGet(string resource);
        dynamic ExecutePost(string resource, object body);
        T ExecuteGet<T>(string resource);
        DownloadResponse DownloadFile(string url);
    }
}