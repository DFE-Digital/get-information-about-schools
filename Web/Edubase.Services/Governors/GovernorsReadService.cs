#if(!TEXAPI)
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
        
        public async Task<ApiSearchResult<SearchGovernorDocument>> SearchAsync(GovernorSearchPayload payload, IPrincipal principal)
        {
            Guard.IsFalse(payload.SortBy == eSortBy.Distance, () => new EdubaseException("Sorting by distance is not supported with Governors"));

            var oDataFilters = new ODataFilterList(ODataFilterList.AND, AzureSearchEndPoint.ODATA_FILTER_DELETED);

            if (payload.Gid != null) oDataFilters.Add(nameof(SearchGovernorDocument.Id), payload.Gid);
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
                        var groupModel = await _groupReadService.GetByEstablishmentUrnAsync(urn.Value, principal);
                        if (groupModel != null && groupModel.GroupTypeId == (int)eLookupGroupType.MultiacademyTrust)
                            retVal.ApplicableRoles.AddRange(new[] { GR.ChairOfLocalGoverningBody, GR.LocalGovernor });
                        else retVal.ApplicableRoles.AddRange(commonGovernorRoleSet);
                    }

                    if (domainModel.GovernanceMode == eGovernanceMode.LocalAndSharedGovernors ||
                        domainModel.GovernanceMode == eGovernanceMode.SharesLocalGovernors ||
                        domainModel.GovernanceMode == eGovernanceMode.NoLocalGovernors)
                    {
                        retVal.ApplicableRoles.AddRange(new [] { GR.SharedChairOfLocalGoverningBody, GR.SharedLocalGovernor });
                    }
                }
            }
            else
            {
                retVal.ApplicableRoles.AddRange(commonGovernorRoleSet);
                var model = await _groupReadService.GetAsync(groupUId.Value, principal);
                if (model.Success)
                {
                    var domainModel = model.GetResult();
                    retVal.HasFullAccess = _securityService.GetEditGroupPermission(principal).CanEdit(groupUId.Value, domainModel.GroupTypeId, domainModel.LocalAuthorityId);
                    if (domainModel.GroupTypeId == (int) eLookupGroupType.MultiacademyTrust)
                    {
                        retVal.ApplicableRoles.AddRange(new[] {GR.SharedChairOfLocalGoverningBody, GR.SharedLocalGovernor});
                    }
                }
            }
            
            var templateDisplayPolicy = new GovernorDisplayPolicy().SetFullAccess(retVal.HasFullAccess);
            retVal.ApplicableRoles.ForEach(x => retVal.RoleDisplayPolicies.Add(x, templateDisplayPolicy.Clone()));
            ProcessDisplayPolicyOverrides(retVal.RoleDisplayPolicies);
            
            retVal.CurrentGovernors = (await GetGovernorsAsync(urn, groupUId, retVal.HasFullAccess, retVal.ApplicableRoles.Cast<int>(), retVal.RoleDisplayPolicies, false)).ToList();
            retVal.HistoricalGovernors = (await GetGovernorsAsync(urn, groupUId, retVal.HasFullAccess, retVal.ApplicableRoles.Cast<int>(), retVal.RoleDisplayPolicies, true)).ToList();

            return retVal;
        }

        /// <summary>
        /// Returns the _Editor_ Display Policy for a given Governor role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public GovernorDisplayPolicy GetEditorDisplayPolicy(GR role, IPrincipal principal)
        {
            var retVal = new GovernorDisplayPolicy().SetFullAccess(true);
            ProcessDisplayPolicyOverrides(new Dictionary<GR, GovernorDisplayPolicy> { [role] = retVal });

            if (role.OneOfThese(GR.AccountingOfficer, GR.ChiefFinancialOfficer))
            {
                // Story 7741: Goverance fields by role.xlsx: ** This is not editable, the date is populated on replacement with the day before the date of appointment of the replacement AO/CFO
                retVal.AppointmentEndDate = false;
            }

            if (role.OneOfThese(GR.SharedChairOfLocalGoverningBody, GR.SharedLocalGovernor))
            {
                retVal.Id = false;
            }

            return retVal;
        }

        private void ProcessDisplayPolicyOverrides(Dictionary<GR, GovernorDisplayPolicy> roleDisplayPolicies)
        {
            // Override policies at the role level
            roleDisplayPolicies.Where(x => x.Key.OneOfThese(GR.Governor, GR.Trustee, GR.LocalGovernor, GR.Member, GR.SharedChairOfLocalGoverningBody, GR.SharedLocalGovernor))
                .ForEach(x => x.Value.EmailAddress = false);

            roleDisplayPolicies.Where(x => x.Key.OneOfThese(GR.AccountingOfficer, GR.ChiefFinancialOfficer)).ForEach(x =>
            {
                x.Value.PostCode = false;
                x.Value.DOB = false;
                x.Value.PreviousFullName = false;
                x.Value.Nationality = false;
                x.Value.TelephoneNumber = false;
                x.Value.AppointingBodyId = false;
                x.Value.AppointmentEndDate = false;
            });

            roleDisplayPolicies.ForEach(kvp =>
            {
                kvp.Value.TelephoneNumber = kvp.Key.OneOfThese(GR.ChairOfGovernors, GR.ChairOfLocalGoverningBody);
            });

            roleDisplayPolicies.Where(x => x.Key.OneOfThese(GR.SharedChairOfLocalGoverningBody, GR.SharedLocalGovernor))
                .ForEach(x =>
                {
                    x.Value.AppointmentStartDate = false;
                    x.Value.AppointmentEndDate = false;
                });
        }

        public async Task<GovernorModel> GetGovernorAsync(int gid, IPrincipal principal)
        {
            var displayPolicy = new GovernorDisplayPolicy().SetFullAccess(true);
            var db = _dbContextFactory.Obtain();
            var governorDataModel = await db.Governors.SingleOrThrowAsync(x => x.Id == gid);
            return Map(governorDataModel, displayPolicy);
        }

        public async Task<IEnumerable<GovernorModel>> GetSharedGovernors(int establishmentUrn)
        {
            var governors = new List<GovernorModel>();

            var groups = await _groupReadService.GetAllByEstablishmentUrnAsync(establishmentUrn);
            
            var templateDisplayPolicy = new GovernorDisplayPolicy().SetFullAccess(true);
            var roles = new List<GR> {GR.SharedChairOfLocalGoverningBody, GR.SharedLocalGovernor};
            var displayPolicies = roles.ToDictionary(r => r, r => templateDisplayPolicy.Clone());
            ProcessDisplayPolicyOverrides(displayPolicies);

            foreach (var group in groups)
            {
                governors.AddRange(await GetGovernorsAsync(null, group.GroupUID, true, roles.Cast<int>(), displayPolicies, false));
            }

            return governors;
        }

        public async Task<SharedGovernorDetailsModel> GetSharedGovernorDetails(int gid)
        {
            var displayPolicy = new GovernorDisplayPolicy().SetFullAccess(true);
            var db = _dbContextFactory.Obtain();
            var governor = await db.Governors.SingleOrThrowAsync(x => x.Id == gid);
            return MapToSharedDetails(governor, displayPolicy);
        }

        private async Task<IEnumerable<GovernorModel>> GetGovernorsAsync(int? urn, int? groupUId, bool fullAccess, IEnumerable<int> roles, Dictionary<GR, GovernorDisplayPolicy> roleDisplayPolicies, bool historic)
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
            return dataModels.Select(governorDataModel =>
            {
                var policy = roleDisplayPolicies.Get((GR)governorDataModel.RoleId.Value);
                Guard.IsNotNull(policy, () => new Exception("The display policy is null!"));
                return Map(governorDataModel, policy);
            });
        }

        private GovernorModel Map(Governor governor, GovernorDisplayPolicy policy)
        {
            return new GovernorModel
            {
                AppointingBodyId = Get(() => governor.AppointingBodyId, policy.AppointingBodyId),
                AppointingBodyName = Get(() => _cachedLookupService.GovernorAppointingBodiesGetAll().FirstOrDefault(l => l.Id == governor.AppointingBodyId)?.Name, policy.AppointingBodyId),
                AppointmentEndDate = Get(() => governor.AppointmentEndDate, true),
                AppointmentStartDate = Get(() => governor.AppointmentStartDate, policy.AppointmentStartDate),
                DOB = Get(() => governor.DOB, policy.DOB),
                RoleId = governor.RoleId,
                EmailAddress = Get(() => governor.EmailAddress, policy.EmailAddress),
                CreatedUtc = governor.CreatedUtc,
                EstablishmentUrn = governor.EstablishmentUrn,
                GroupUID = governor.GroupUID,
                Id = Get(() => governor.Id, policy.Id),
                IsDeleted = governor.IsDeleted,
                LastUpdatedUtc = governor.LastUpdatedUtc,
                Nationality = Get(() => governor.Nationality, policy.Nationality),
                Person_FirstName = Get(() => governor.Person.FirstName, policy.FullName),
                Person_LastName = Get(() => governor.Person.LastName, policy.FullName),
                Person_MiddleName = Get(() => governor.Person.MiddleName, policy.FullName),
                Person_Title = Get(() => governor.Person.Title, policy.FullName),
                PostCode = Get(() => governor.PostCode, policy.PostCode),
                PreviousPerson_FirstName = Get(() => governor.PreviousPerson.FirstName, policy.PreviousFullName),
                PreviousPerson_LastName = Get(() => governor.PreviousPerson.LastName, policy.PreviousFullName),
                PreviousPerson_MiddleName = Get(() => governor.PreviousPerson.MiddleName, policy.PreviousFullName),
                PreviousPerson_Title = Get(() => governor.PreviousPerson.Title, policy.PreviousFullName),
                TelephoneNumber = Get(() => governor.TelephoneNumber, policy.TelephoneNumber)
            };
        }

        private SharedGovernorDetailsModel MapToSharedDetails(Governor governor, GovernorDisplayPolicy policy)
        {
            return new SharedGovernorDetailsModel
            {
                AppointingBodyId = Get(() => governor.AppointingBodyId, policy.AppointingBodyId),
                AppointingBodyName = Get(() => _cachedLookupService.GovernorAppointingBodiesGetAll().FirstOrDefault(l => l.Id == governor.AppointingBodyId)?.Name, policy.AppointingBodyId),
                AppointmentEndDate = Get(() => governor.AppointmentEndDate, true),
                AppointmentStartDate = Get(() => governor.AppointmentStartDate, policy.AppointmentStartDate),
                DOB = Get(() => governor.DOB, policy.DOB),
                RoleId = governor.RoleId,
                EmailAddress = Get(() => governor.EmailAddress, policy.EmailAddress),
                CreatedUtc = governor.CreatedUtc,
                EstablishmentUrn = governor.EstablishmentUrn,
                GroupUID = governor.GroupUID,
                Id = Get(() => governor.Id, policy.Id),
                IsDeleted = governor.IsDeleted,
                LastUpdatedUtc = governor.LastUpdatedUtc,
                Nationality = Get(() => governor.Nationality, policy.Nationality),
                Person_FirstName = Get(() => governor.Person.FirstName, policy.FullName),
                Person_LastName = Get(() => governor.Person.LastName, policy.FullName),
                Person_MiddleName = Get(() => governor.Person.MiddleName, policy.FullName),
                Person_Title = Get(() => governor.Person.Title, policy.FullName),
                PostCode = Get(() => governor.PostCode, policy.PostCode),
                PreviousPerson_FirstName = Get(() => governor.PreviousPerson.FirstName, policy.PreviousFullName),
                PreviousPerson_LastName = Get(() => governor.PreviousPerson.LastName, policy.PreviousFullName),
                PreviousPerson_MiddleName = Get(() => governor.PreviousPerson.MiddleName, policy.PreviousFullName),
                PreviousPerson_Title = Get(() => governor.PreviousPerson.Title, policy.PreviousFullName),
                TelephoneNumber = Get(() => governor.TelephoneNumber, policy.TelephoneNumber),
                Appointments = governor.Establishments.Select(e => new GovernorAppointment { AppointmentStartDate = e.AppointmentEndDate.Value, AppointmentEndDate = e.AppointmentEndDate.Value, EstablishmentUrn = e.EstabishmentUrn })
            };
        }

        private T Get<T>(Func<T> func, bool flag)
        {
            if (flag) return func();
            else return default(T);
        }


    }
}

#endif