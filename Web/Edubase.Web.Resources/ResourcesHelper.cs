using System.Security.Principal;
using Edubase.Services.Enums;

namespace Edubase.Web.Resources
{
    public class ResourcesHelper : IResourcesHelper
    {
        public string GetResourceStringForEstablishment(string name, eLookupEstablishmentTypeGroup? establishmentType, IPrincipal user)
        {
            if (user != null && user.Identity.IsAuthenticated && establishmentType != null)
            {
                switch (establishmentType.Value)
                {
                    case eLookupEstablishmentTypeGroup.Academies:
                        return Academy.ResourceManager.GetString(name);
                    case eLookupEstablishmentTypeGroup.LAMaintainedSchools:
                        return LocalAuthority.ResourceManager.GetString(name);
                }
            }

            return Public.ResourceManager.GetString(name);
        }
    }
}
