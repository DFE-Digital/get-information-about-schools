namespace Edubase.AcceptanceTests.Apis
{
    public interface IApi
    {
        Task<T> GetAsync<T>(string url);
    }
}
