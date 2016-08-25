using Web.Services.Api;

namespace Web.Services.Schools
{
    public class SchoolService : ISchoolService
    {
        private readonly ISchoolPermissions _schoolPermissions;
        private readonly IApiService _apiService;

        public SchoolService(ISchoolPermissions schoolPermissions, IApiService apiService)
        {
            _schoolPermissions = schoolPermissions;
            _apiService = apiService;
        }

        public dynamic GetSchoolDetails(int id)
        {
            _schoolPermissions.EnsureHasAccessToSchool(id);
            return _apiService.GetSchoolById(id.ToString());
        }
    }
}