using System.Collections.Generic;
using System.Linq;
using Web.Domain;
using Web.Services;

namespace Web.UI.Models
{
    public class SiteContext : ISiteContext
    {
        private readonly ISchoolPermissions _schoolPermissions;

        public SiteContext(ISchoolPermissions schoolPermissions)
        {
            _schoolPermissions = schoolPermissions;
        }

        public Permissions Permissions => new Permissions(_schoolPermissions.GetAccessibleSchoolIds().ToArray());

        public void SetAllowedSchools(ISchoolService schoolService)
        {
            
        }

        public SchoolDetails CurrentSchool { get; }
        public IEnumerable<SchoolDetails> AllowedSchools { get; } 
    }

    public class Permissions
    {
        public Permissions(IEnumerable<int> allowedIds)
        {
            AllowedSchoolIds = allowedIds;
        }

        public bool CanViewAllSchools
        {
            get { return AllowedSchoolIds.Any(x => x == -1); }
        }

        public bool AllowedSingleSchoolOnly => AllowedSchoolIds.Count() == 1;
        public int SingleSchoolId => AllowedSchoolIds.First();

        public bool CanAccessSchool(int id)
        {
            return AllowedSchoolIds.Contains(id);
        }

        public IEnumerable<int> AllowedSchoolIds { get; }
    }
}