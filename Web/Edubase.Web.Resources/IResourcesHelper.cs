using System.Security.Principal;
using Edubase.Services.Enums;

namespace Edubase.Web.Resources
{
    public interface IResourcesHelper
    {
        string GetResourceStringForEstablishment(string name, eLookupEstablishmentTypeGroup? establishmentType, IPrincipal user);
    }
}
