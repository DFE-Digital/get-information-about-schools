using System.Collections.Generic;

namespace Edubase.Web.UI.Identity
{
    public interface ISchoolPermissions
    {
        IEnumerable<int> GetAccessibleSchoolIds();
        void EnsureHasAccessToSchool(int id);
    }
}