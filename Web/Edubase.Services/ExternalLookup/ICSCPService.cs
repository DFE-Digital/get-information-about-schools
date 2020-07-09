namespace Edubase.Services.ExternalLookup
{
    public interface ICSCPService
    {
        bool CheckExists(int? urn, string name);
        string SchoolURL(int? urn, string name);
    }
}
