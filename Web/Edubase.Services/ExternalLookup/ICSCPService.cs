namespace Edubase.Services.ExternalLookup
{
    public interface ICSCPService
    {
        bool CheckExists(int? urn);
        string SchoolURL(int? urn);
    }
}
