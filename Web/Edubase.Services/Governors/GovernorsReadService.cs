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
using MoreLinq;
using Edubase.Services.Exceptions;
using Edubase.Services.Core.Search;
using Edubase.Services.Governors.Models;
using Edubase.Services.Security;
using Edubase.Services.Establishments;
using Edubase.Services.Groups;
using Edubase.Services.Enums;

namespace Edubase.Services.Governors
{
    using DisplayPolicies;
    using GR = eLookupGovernorRole;

    public class GovernorsReadService : IGovernorsReadService
    {
        private readonly IAzureSearchEndPoint _azureSearchService;
        private readonly ISecurityService _securityService;
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IGroupReadService _groupReadService;
        private readonly IApplicationDbContextFactory _dbContextFactory;

        public GovernorsReadService(
            IAzureSearchEndPoint azureSearchService, 
            ISecurityService securityService,
            IEstablishmentReadService establishmentReadService,
            IGroupReadService groupReadService,
            IApplicationDbContextFactory dbContextFactory)
        {
            _azureSearchService = azureSearchService;
            _securityService = securityService;
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
            _dbContextFactory = dbContextFactory;
        }

        public async Task<IEnumerable<Governor>> GetCurrentByUrn(int urn)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<Governor>> GetCurrentByGroupUID(int groupUID)
        {
            throw new NotImplementedException();
        }


        public async Task<IEnumerable<Governor>> GetHistoricalByUrn(int urn)
        {
            throw new NotImplementedException();
        }


        public async Task<IEnumerable<Governor>> GetHistoricalByGroupUID(int groupUID)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Governors who haven't left yet
        /// </summary>
        /// <param name="dc"></param>
        /// <returns></returns>
        private IQueryable<Governor> GetCurrentQuery(ApplicationDbContext dc)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Governors who left in the past 12 months
        /// </summary>
        /// <param name="dc"></param>
        /// <returns></returns>
        private IQueryable<Governor> GetHistoricalQuery(ApplicationDbContext dc)
        {
            throw new NotImplementedException();
        }

        public async Task<AzureSearchResult<SearchGovernorDocument>> SearchAsync(GovernorSearchPayload payload)
        {
            Guard.IsFalse(payload.SortBy == eSortBy.Distance, () => new EdubaseException("Sorting by distance is not supported with Governors"));

            var oDataFilters = new ODataFilterList(ODataFilterList.AND, AzureSearchEndPoint.ODATA_FILTER_DELETED);

            if (payload.FirstName.Clean() != null) oDataFilters.Add(nameof(SearchGovernorDocument.Person_FirstNameDistilled), payload.FirstName.Distill());
            if (payload.LastName.Clean() != null) oDataFilters.Add(nameof(SearchGovernorDocument.Person_LastNameDistilled), payload.LastName.Distill());
            
            var date = payload.IncludeHistoric ? DateTime.UtcNow.Date.AddYears(-1) : DateTime.UtcNow.Date;
            var appointmentEndDateFilter = new ODataFilterList(ODataFilterList.OR);
            appointmentEndDateFilter.Add(nameof(SearchGovernorDocument.AppointmentEndDate), null);
            appointmentEndDateFilter.Add(nameof(SearchGovernorDocument.AppointmentEndDate), date, ODataFilterList.GE);
            oDataFilters.Add(appointmentEndDateFilter);

            if (payload.RoleIds.Any())
            {
                var roleIdODataFilter = new ODataFilterList(ODataFilterList.OR);
                payload.RoleIds.ForEach(x => roleIdODataFilter.Add(nameof(SearchGovernorDocument.RoleId), x));
                oDataFilters.Add(roleIdODataFilter);
            }

            return await _azureSearchService.SearchAsync<SearchGovernorDocument>(GovernorsSearchIndex.INDEX_NAME,
                null,
                oDataFilters.ToString(),
                payload.Skip,
                payload.Take,
                new[] { nameof(SearchGovernorDocument.Person_LastName) }.ToList(),
                ODataUtil.OrderBy(nameof(SearchGovernorDocument.Person_LastNameDistilled), (payload.SortBy == eSortBy.NameAlphabeticalAZ)));
        }

        /// <summary>
        /// Gets the governor list for a group or establishment, together with the Governor Display Policy and the list of applicable roles.
        /// </summary>
        /// <param name="urn"></param>
        /// <param name="groupUId"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        public async Task<GovernorsListDto> GetGovernorListAsync(int? urn = null, int? groupUId = null, IPrincipal principal = null)
        {
            Guard.IsTrue(urn.HasValue || groupUId.HasValue, () => new ArgumentNullException(urn.HasValue ? nameof(urn) : nameof(groupUId)));
            Guard.IsNotNull(principal, () => new ArgumentNullException(nameof(principal)));

            var retVal = new GovernorsListDto();
            var governorDisplayPolicy = new GovernorDisplayPolicy();
            var applicableGovernorRoles = new List<GR>();
            var commonGovernorRoleSet = new[] { GR.ChairOfTrustees, GR.Trustee, GR.Member, GR.AccountingOfficer, GR.ChiefFinancialOfficer }; // set used by Academies & Free Schools NOT in a MAT and Multi-Academy Trusts

            bool fullAccess = false; // whether the user can view a subset of the governor information or the full data set

            if (urn.HasValue)
            {
                var model = await _establishmentReadService.GetAsync(urn.Value, principal);
                if (model.Success)
                {
                    var domainModel = model.GetResult();
                    fullAccess = _securityService.GetEditEstablishmentPermission(principal).CanEdit(urn.Value, domainModel.TypeId, null, domainModel.LocalAuthorityId, domainModel.EstablishmentTypeGroupId);

                    if (EnumSets.LAMaintainedEstablishments.Any(x => x == domainModel.TypeId))
                        applicableGovernorRoles.AddRange(new[] { GR.ChairOfGovernors, GR.Governor });
                    else if (EnumSets.AcademiesAndFreeSchools.Any(x => x == domainModel.TypeId))
                    {
                        var groupModel = await _groupReadService.GetByEstablishmentUrnAsync(urn.Value);
                        if (groupModel != null && groupModel.GroupTypeId == (int)eLookupGroupType.MultiacademyTrust)
                            applicableGovernorRoles.AddRange(new[] { GR.ChairOfLocalGoverningBody, GR.LocalGovernor });
                        else applicableGovernorRoles.AddRange(commonGovernorRoleSet);
                    }
                }
            }
            else
            {
                var model = await _groupReadService.GetAsync(groupUId.Value, principal);
                if (model.Success)
                {
                    var domainModel = model.GetResult();
                    fullAccess = _securityService.GetEditGroupPermission(principal).CanEdit(groupUId.Value, domainModel.GroupTypeId, domainModel.LocalAuthorityId);
                }
                applicableGovernorRoles.AddRange(commonGovernorRoleSet);
            }
            
            
            retVal.CurrentGovernors = await GetGovernorsAsync(urn, groupUId, fullAccess, applicableGovernorRoles.Cast<int>(), false);
            retVal.HistoricGovernors = await GetGovernorsAsync(urn, groupUId, fullAccess, applicableGovernorRoles.Cast<int>(), true);
            retVal.ApplicableRoles = applicableGovernorRoles;
            retVal.DisplayPolicy = governorDisplayPolicy.SetFullAccess(fullAccess);

            return retVal;
        }

        private async Task<IEnumerable<GovernorModel>> GetGovernorsAsync(int? urn, int? groupUId, bool fullAccess, IEnumerable<int> roles, bool historic)
        {
            var db = _dbContextFactory.Obtain();

            var governorsList = Enumerable.Empty<GovernorModel>();
            var query = db.Governors.Where(x => (x.EstablishmentUrn == urn || x.GroupUID == groupUId) && x.RoleId != null && roles.Contains(x.RoleId.Value) && x.IsDeleted == false);

            var today = DateTime.Now.Date;
            if (historic)
            {
                var oneYearAgo = DateTime.Now.Date.AddYears(-1);
                query = query.Where(x => x.AppointmentEndDate > oneYearAgo && x.AppointmentEndDate < today);
            }
            else query = query.Where(x => x.AppointmentEndDate > today || x.AppointmentEndDate == null);

            if (fullAccess)
            {
                governorsList = await query.Select(x =>
                    new GovernorModel
                    {
                        RoleId = x.RoleId,
                        Id = x.Id,
                        Person_Title = x.Person.Title,
                        Person_FirstName = x.Person.FirstName,
                        Person_MiddleName = x.Person.MiddleName,
                        Person_LastName = x.Person.LastName,
                        AppointingBodyId = x.AppointingBodyId,
                        AppointmentStartDate = x.AppointmentStartDate,
                        AppointmentEndDate = x.AppointmentEndDate,
                        PostCode = x.PostCode,
                        EmailAddress = x.EmailAddress,
                        DOB = x.DOB,
                        PreviousPerson_Title = x.PreviousPerson.Title,
                        PreviousPerson_FirstName = x.PreviousPerson.FirstName,
                        PreviousPerson_MiddleName = x.PreviousPerson.MiddleName,
                        PreviousPerson_LastName = x.PreviousPerson.LastName,
                        Nationality = x.Nationality
                    }).ToListAsync();
            }
            else
            {
                governorsList = await query.Select(x =>
                    new GovernorModel
                    {
                        RoleId = x.RoleId,
                        Person_Title = x.Person.Title,
                        Person_FirstName = x.Person.FirstName,
                        Person_MiddleName = x.Person.MiddleName,
                        Person_LastName = x.Person.LastName,
                        AppointingBodyId = x.AppointingBodyId,
                        AppointmentStartDate = x.AppointmentStartDate,
                        AppointmentEndDate = x.AppointmentEndDate
                    }).ToListAsync();
            }

            return governorsList;
        }
    }
}
