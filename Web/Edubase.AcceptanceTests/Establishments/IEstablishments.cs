namespace Edubase.AcceptanceTests.Establishments
{
    public interface IEstablishments
    {
        Task<Establishment> GetEstablishment(int urn);
    }
}
