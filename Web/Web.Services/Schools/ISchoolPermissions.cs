using System.Collections.Generic;

namespace Web.Services.Schools
{
    public interface ISchoolPermissions
    {
        IEnumerable<int> GetAccessibleSchoolIds();
        void EnsureHasAccessToSchool(int id);
    }
}