namespace Web.Services.Api
{
    public interface IAppSettings
    {
        string ApiBaseUrl { get; }
        string ApiUserName { get; }
        string ApiPassword { get; }
    }
}