namespace Edubase.Services.ExternalLookup
{
    public interface IFBService
    {
        bool CheckExists(int? urn);
        string SchoolURL(int? urn);
    }
}
