namespace Edubase.AcceptanceTests.GiasFrontEnd
{
    public interface IApi
    {
        Task<T> GetAsync<T>(string url);
    }
}
