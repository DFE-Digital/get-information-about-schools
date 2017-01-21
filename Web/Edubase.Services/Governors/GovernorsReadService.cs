using Edubase.Common;
using Edubase.Data;
using Edubase.Data.DbContext;
using Edubase.Data.Entity;
using Edubase.Services.Governors.Search;
using Edubase.Services.IntegrationEndPoints.AzureSearch;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Governors
{
    public class GovernorsReadService : IGovernorsReadService
    {
        IAzureSearchEndPoint _azureSearchService;

        public GovernorsReadService(IAzureSearchEndPoint azureSearchService)
        {
            _azureSearchService = azureSearchService;
        }

        public async Task<IEnumerable<Governor>> GetCurrentByUrn(int urn)
        {
            return await ApplicationDbContext
                .OperationAsync(dc => GetCurrentQuery(dc)
                .Where(x => x.EstablishmentUrn == urn).ToArrayAsync());
        }
        public async Task<IEnumerable<Governor>> GetCurrentByGroupUID(int groupUID)
        {
            return await ApplicationDbContext
                .OperationAsync(dc => GetCurrentQuery(dc)
                .Where(x => x.GroupUID == groupUID).ToArrayAsync());
        }


        public async Task<IEnumerable<Governor>> GetHistoricalByUrn(int urn)
        {
            return await ApplicationDbContext
                .OperationAsync(dc => GetHistoricalQuery(dc)
                .Where(x => x.EstablishmentUrn == urn).ToArrayAsync());
        }


        public async Task<IEnumerable<Governor>> GetHistoricalByGroupUID(int groupUID)
        {
            return await ApplicationDbContext
                .OperationAsync(dc => GetHistoricalQuery(dc)
                .Where(x => x.GroupUID == groupUID).ToArrayAsync());
        }
        /// <summary>
        /// Governors who haven't left yet
        /// </summary>
        /// <param name="dc"></param>
        /// <returns></returns>
        private IQueryable<Governor> GetCurrentQuery(ApplicationDbContext dc)
        {
            var date = DateTime.UtcNow.Date;
            return dc.Governors
                .Include(x => x.AppointingBody).Include(x => x.Role)
                .Where(x => (!x.AppointmentEndDate.HasValue || x.AppointmentEndDate > date) && x.IsDeleted == false);
        }

        /// <summary>
        /// Governors who left in the past 12 months
        /// </summary>
        /// <param name="dc"></param>
        /// <returns></returns>
        private IQueryable<Governor> GetHistoricalQuery(ApplicationDbContext dc)
        {
            var date1 = DateTime.UtcNow.Date.AddYears(-1);
            var date2 = DateTime.UtcNow.Date;
            return dc.Governors.Include(x => x.AppointingBody).Include(x => x.Role)
                .Where(x => (x.AppointmentEndDate != null
                && x.AppointmentEndDate.Value > date1
                && x.AppointmentEndDate < date2) && x.IsDeleted == false);
        }

        public async Task<AzureSearchResult<SearchGovernorDocument>> SearchAsync(GovernorSearchPayload payload, IPrincipal principal)
        {
            var oDataFilters = new ODataFilterList(ODataFilterList.AND, AzureSearchEndPoint.ODATA_FILTER_DELETED);

            Func<string, string> titlise = System.Globalization.CultureInfo.CurrentUICulture.TextInfo.ToTitleCase;
            if (payload.FirstName.Clean() != null) oDataFilters.Add(nameof(SearchGovernorDocument.Person_FirstName), titlise(payload.FirstName.ToLower()));
            if (payload.LastName.Clean() != null) oDataFilters.Add(nameof(SearchGovernorDocument.Person_LastName), titlise(payload.LastName.ToLower()));
            if (payload.RoleId.HasValue) oDataFilters.Add(nameof(SearchGovernorDocument.RoleId), payload.RoleId);

            var date = payload.IncludeHistoric ? DateTime.UtcNow.Date.AddYears(-1) : DateTime.UtcNow.Date;

            var appointmentEndDateFilter = new ODataFilterList(ODataFilterList.OR);
            appointmentEndDateFilter.Add(nameof(SearchGovernorDocument.AppointmentEndDate), null);
            appointmentEndDateFilter.Add(nameof(SearchGovernorDocument.AppointmentEndDate), date, ODataFilterList.GE);
            oDataFilters.Add(appointmentEndDateFilter);
            
            return await _azureSearchService.SearchAsync<SearchGovernorDocument>(GovernorsSearchIndex.INDEX_NAME,
                null,
                oDataFilters.ToString(),
                payload.Skip,
                payload.Take,
                new[] { nameof(SearchGovernorDocument.Person_LastName) }.ToList(),
                payload.OrderBy);
        }
    }
}
