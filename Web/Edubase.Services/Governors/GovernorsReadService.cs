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
    using Lookup;
    using System.Linq.Expressions;
    using GR = eLookupGovernorRole;

    public class GovernorsReadService : IGovernorsReadService
    {
        private readonly IAzureSearchEndPoint _azureSearchService;
        private readonly ISecurityService _securityService;
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IGroupReadService _groupReadService;
        private readonly IApplicationDbContextFactory _dbContextFactory;
        private readonly ICachedLookupService _cachedLookupService;

        public GovernorsReadService(
            IAzureSearchEndPoint azureSearchService, 
            ISecurityService securityService,
            IEstablishmentReadService establishmentReadService,
            IGroupReadService groupReadService,
            IApplicationDbContextFactory dbContextFactory,
            ICachedLookupService cachedLookupService)
        {
            _azureSearchService = azureSearchService;
            _securityService = securityService;
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
            _dbContextFactory = dbContextFactory;
            _cachedLookupService = cachedLookupService;
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
        public async Task<GovernorsDetailsDto> GetGovernorListAsync(int? urn = null, int? groupUId = null, IPrincipal principal = null)
        {
            Guard.IsTrue(urn.HasValue || groupUId.HasValue, () => new ArgumentNullException(urn.HasValue ? nameof(urn) : nameof(groupUId)));
            Guard.IsNotNull(principal, () => new ArgumentNullException(nameof(principal)));

            var retVal = new GovernorsDetailsDto();
            var commonGovernorRoleSet = new[] { GR.ChairOfTrustees, GR.Trustee, GR.Member, GR.AccountingOfficer, GR.ChiefFinancialOfficer }; // set used by Academies & Free Schools NOT in a MAT and Multi-Academy Trusts
            
            if (urn.HasValue)
            {
                var model = await _establishmentReadService.GetAsync(urn.Value, principal);
                if (model.Success)
                {
                    var domainModel = model.GetResult();
                    retVal.HasFullAccess = _securityService.GetEditEstablishmentPermission(principal).CanEdit(urn.Value, domainModel.TypeId, null, domainModel.LocalAuthorityId, domainModel.EstablishmentTypeGroupId);

                    if (EnumSets.LAMaintainedEstablishments.Any(x => x == domainModel.TypeId))
                        retVal.ApplicableRoles.AddRange(new[] { GR.ChairOfGovernors, GR.Governor });
                    else if (EnumSets.AcademiesAndFreeSchools.Any(x => x == domainModel.TypeId))
                    {
                        var groupModel = await _groupReadService.GetByEstablishmentUrnAsync(urn.Value);
                        if (groupModel != null && groupModel.GroupTypeId == (int)eLookupGroupType.MultiacademyTrust)
                            retVal.ApplicableRoles.AddRange(new[] { GR.ChairOfLocalGoverningBody, GR.LocalGovernor });
                        else retVal.ApplicableRoles.AddRange(commonGovernorRoleSet);
                    }
                }
            }
            else
            {
                var model = await _groupReadService.GetAsync(groupUId.Value, principal);
                if (model.Success)
                {
                    var domainModel = model.GetResult();
                    retVal.HasFullAccess = _securityService.GetEditGroupPermission(principal).CanEdit(groupUId.Value, domainModel.GroupTypeId, domainModel.LocalAuthorityId);
                }
                retVal.ApplicableRoles.AddRange(commonGovernorRoleSet);
            }
            
            var templateDisplayPolicy = new GovernorDisplayPolicy().SetFullAccess(retVal.HasFullAccess);
            retVal.ApplicableRoles.ForEach(x => retVal.RoleDisplayPolicies.Add(x, templateDisplayPolicy.Clone()));
            ProcessDisplayPolicyOverrides(retVal.RoleDisplayPolicies);
            
            retVal.CurrentGovernors = await GetGovernorsAsync(urn, groupUId, retVal.HasFullAccess, retVal.ApplicableRoles.Cast<int>(), retVal.RoleDisplayPolicies, false);
            retVal.HistoricGovernors = await GetGovernorsAsync(urn, groupUId, retVal.HasFullAccess, retVal.ApplicableRoles.Cast<int>(), retVal.RoleDisplayPolicies, true);

            return retVal;
        }

        /// <summary>
        /// Returns the _Editor_ Display Policy for a given Governor role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public GovernorDisplayPolicy GetEditorDisplayPolicy(GR role)
        {
            var retVal = new GovernorDisplayPolicy().SetFullAccess(true);
            ProcessDisplayPolicyOverrides(new Dictionary<GR, GovernorDisplayPolicy> { [role] = retVal });

            retVal.AppointmentEndDate = !(role.OneOfThese(GR.AccountingOfficer, GR.ChiefFinancialOfficer)); // Story 7741: Goverance fields by role.xlsx: ** This is not editable, the date is populated on replacement with the day before the date of appointment of the replacement AO/CFO

            return retVal;
        }

        private void ProcessDisplayPolicyOverrides(Dictionary<GR, GovernorDisplayPolicy> roleDisplayPolicies)
        {
            // Override policies at the role level
            roleDisplayPolicies.Where(x => x.Key.OneOfThese(GR.Governor, GR.Trustee, GR.LocalGovernor, GR.Member))
                .ForEach(x => x.Value.EmailAddress = false);

            roleDisplayPolicies.Where(x => x.Key.OneOfThese(GR.AccountingOfficer, GR.ChiefFinancialOfficer)).ForEach(x =>
            {
                x.Value.PostCode = false;
                x.Value.DOB = false;
                x.Value.PreviousFullName = false;
                x.Value.Nationality = false;
            });
        }

        private async Task<IEnumerable<GovernorModel>> GetGovernorsAsync(int? urn, int? groupUId, bool fullAccess, IEnumerable<int> roles, Dictionary<eLookupGovernorRole, GovernorDisplayPolicy> roleDisplayPolicies, bool historic)
        {
            var db = _dbContextFactory.Obtain();
            var query = db.Governors.Where(x => (urn != null && x.EstablishmentUrn == urn || groupUId != null && x.GroupUID == groupUId) && x.RoleId != null && roles.Contains(x.RoleId.Value) && x.IsDeleted == false);

            var today = DateTime.Now.Date;
            if (historic)
            {
                var oneYearAgo = DateTime.Now.Date.AddYears(-1);
                query = query.Where(x => x.AppointmentEndDate > oneYearAgo && x.AppointmentEndDate < today);
            }
            else query = query.Where(x => x.AppointmentEndDate > today || x.AppointmentEndDate == null);

            var dataModels = await query.ToListAsync();
            return dataModels.Select(x =>
            {
                var p = roleDisplayPolicies.Get((GR)x.RoleId.Value);
                Guard.IsNotNull(p, () => new Exception("The display policy is null!"));
                return new GovernorModel
                {
                    AppointingBodyId = Get(() => x.AppointingBodyId, p.AppointingBodyId),
                    AppointingBodyName = Get(() => _cachedLookupService.GovernorAppointingBodiesGetAll().FirstOrDefault(l => l.Id == x.AppointingBodyId)?.Name, p.AppointingBodyId),
                    AppointmentEndDate = Get(() => x.AppointmentEndDate, p.AppointmentEndDate),
                    AppointmentStartDate = Get(() => x.AppointmentStartDate, p.AppointmentStartDate),
                    DOB = Get(() => x.DOB, p.DOB),
                    RoleId = x.RoleId,
                    EmailAddress = Get(() => x.EmailAddress, p.EmailAddress),
                    CreatedUtc = x.CreatedUtc,
                    EstablishmentUrn = x.EstablishmentUrn,
                    GroupUID = x.GroupUID,
                    Id = Get(() => x.Id, p.Id),
                    IsDeleted = x.IsDeleted,
                    LastUpdatedUtc = x.LastUpdatedUtc,
                    Nationality = Get(() => x.Nationality, p.Nationality),
                    Person_FirstName = Get(() => x.Person.FirstName, p.FullName),
                    Person_LastName = Get(() => x.Person.LastName, p.FullName),
                    Person_MiddleName = Get(() => x.Person.MiddleName, p.FullName),
                    Person_Title = Get(() => x.Person.Title, p.FullName),
                    PostCode = Get(() => x.PostCode, p.PostCode),
                    PreviousPerson_FirstName = Get(() => x.PreviousPerson.FirstName, p.PreviousFullName),
                    PreviousPerson_LastName = Get(() => x.PreviousPerson.LastName, p.PreviousFullName),
                    PreviousPerson_MiddleName = Get(() => x.PreviousPerson.MiddleName, p.PreviousFullName),
                    PreviousPerson_Title = Get(() => x.PreviousPerson.Title, p.PreviousFullName)
                };
            });
        }

        private T Get<T>(Func<T> func, bool flag)
        {
            if (flag) return func();
            else return default(T);
        }
    }
}
