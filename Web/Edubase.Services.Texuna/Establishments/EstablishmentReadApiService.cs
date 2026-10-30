using Edubase.Common;
using Edubase.Common.Reflection;
using Edubase.Services.Core;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Lookup;
using Edubase.Services.Texuna.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using MoreLinq;
using Edubase.Services.Establishments.EditPolicies;

namespace Edubase.Services.Texuna.Establishments
{
    using EP = eLookupEducationPhase;
    using ET = eLookupEstablishmentType;

    /// <summary>
    /// Implementation of IEstablishmentReadService that will gradually be changed to call the API rather than custom backend
    /// </summary>
    public class EstablishmentReadApiService : IEstablishmentReadService
    {
        private const string ApiSuggestPath = "suggest/establishment";

        private readonly ICachedLookupService _cachedLookupService;
        private readonly IHttpClientWrapper _httpClient;

        public EstablishmentReadApiService(IHttpClientWrapper httpClient, ICachedLookupService cachedLookupService)
        {
            _httpClient = httpClient;
            _cachedLookupService = cachedLookupService;
        }

        public async Task<ServiceResultDto<bool>> CanAccess(int urn, IPrincipal principal)
          => new ServiceResultDto<bool>((await _httpClient.GetAsync<BoolResult>($"establishment/{urn}/canaccess", principal)).GetResponse().Value);

        public async Task<bool> CanEditAsync(int urn, IPrincipal principal)
            => (await _httpClient.GetAsync<BoolResult>($"establishment/{urn}/canedit", principal)).Response.Value;

        public async Task<IEnumerable<AddressLookupResult>> GetAddressesByPostCodeAsync(string postCode, IPrincipal principal)
        {
            try
            {
                var list = await _httpClient.GetAsync<AddressBaseResult[]>("establishment/addressBase/queryByPostcode?postcode=" + postCode.Replace(" ", string.Empty), principal);
                return list.GetResponse().Select(x => new AddressLookupResult(x)).ToList();
            }
            catch // I have actually given up asking Texuna to return errors in the error envelope at this stage.
            {
                return Enumerable.Empty<AddressLookupResult>();
            }
        }

        public async Task<ServiceResultDto<EstablishmentModel>> GetAsync(int urn, IPrincipal principal)
                    => new ServiceResultDto<EstablishmentModel>((await _httpClient.GetAsync<EstablishmentModel>($"establishment/{urn}", principal, false)).Response);

        public async Task<PaginatedResult<EstablishmentChangeDto>> GetChangeHistoryAsync(int urn, int skip, int take, string sortBy, IPrincipal user)
        {
            var changes = (await _httpClient.GetAsync<ApiPagedResult<EstablishmentChangeDto>>($"establishment/{urn}/changes?skip={skip}&take={take}&sortby={sortBy}", user)).GetResponse();
            return new PaginatedResult<EstablishmentChangeDto>(skip, take, changes.Count, changes.Items);
        }

        public async Task<PaginatedResult<EstablishmentChangeDto>> GetChangeHistoryAsync(int urn, int skip, int take, string sortBy, EstablishmentChangeHistoryFilters filters, IPrincipal user)
        {
            var changes = (await _httpClient.PostAsync<ApiPagedResult<EstablishmentChangeDto>>($"establishment/{urn}/changes?skip={skip}&take={take}&sortby={sortBy}", filters, user)).GetResponse();
            return new PaginatedResult<EstablishmentChangeDto>(skip, take, changes.Count, changes.Items);
        }

        public async Task<FileDownloadDto> GetChangeHistoryDownloadAsync(int urn, DownloadType format, IPrincipal principal)
            => (await _httpClient.GetAsync<FileDownloadDto>($"establishment/{urn}/changes/download?format={format.ToString().ToLower()}", principal)).GetResponse();

        public async Task<FileDownloadDto> GetChangeHistoryDownloadAsync(int urn, EstablishmentChangeHistoryDownloadFilters filters, IPrincipal principal)
            => (await _httpClient.PostAsync<FileDownloadDto>($"establishment/{urn}/changes/download", filters, principal)).GetResponse();

        public async Task<PaginatedResult<EstablishmentChangeDto>> GetGovernanceChangeHistoryAsync(int urn, int skip, int take, string sortBy, IPrincipal user)
        {
            var changes = (await _httpClient.GetAsync<ApiPagedResult<EstablishmentChangeDto>>($"establishment/{urn}/governance/changes?skip={skip}&take={take}&sortby={sortBy}", user)).GetResponse();
            return new PaginatedResult<EstablishmentChangeDto>(skip, take, changes.Count, changes.Items);
        }

        public async Task<FileDownloadDto> GetGovernanceChangeHistoryDownloadAsync(int urn, DownloadType format, IPrincipal principal)
            => (await _httpClient.GetAsync<FileDownloadDto>($"establishment/{urn}/governance/changes/download?format={format.ToString().ToLower()}", principal)).GetResponse();

        public async Task<EstablishmentDisplayEditPolicy> GetDisplayPolicyAsync(EstablishmentModel establishment, IPrincipal user)
                            => (await _httpClient.GetAsync<EstablishmentDisplayEditPolicy>($"establishment/{establishment.Urn}/display-policy", user)).GetResponse().Initialise(establishment);

        public async Task<FileDownloadDto> GetDownloadAsync(int urn, DownloadType format, IPrincipal principal)
            => (await _httpClient.GetAsync<FileDownloadDto>($"establishment/{urn}/download?format={format.ToString().ToLower()}", principal)).GetResponse();

        public async Task<EstablishmentEditPolicyEnvelope> GetEditPolicyAsync(EstablishmentModel establishment, IPrincipal user)
                    => (await _httpClient.GetAsync<EstablishmentEditPolicyEnvelope>($"establishment/{establishment.Urn}/edit-policy", user)).GetResponse().Initialise(establishment);

        public async Task<EstablishmentEditPolicyEnvelope> GetEditPolicyByUrnAsync(int urn, IPrincipal user)
                    => (await _httpClient.GetAsync<EstablishmentEditPolicyEnvelope>($"establishment/{urn}/edit-policy", user)).GetResponse();

        public async Task<string> GetEstablishmentNameAsync(int urn, IPrincipal principal) => (await GetAsync(urn, principal)).GetResult().Name;

        public Dictionary<ET, EP[]> GetEstabType2EducationPhaseMap()
        {
            var retVal = new Dictionary<ET, EP[]>
            {
                { ET.CommunitySchool, new[] { EP.Nursery, EP.Primary, EP.MiddleDeemedPrimary, EP.Secondary, EP.MiddleDeemedSecondary, EP._16Plus, EP.AllThrough } },
                { ET.VoluntaryAidedSchool, new[] { EP.Primary, EP.MiddleDeemedPrimary, EP.Secondary, EP.MiddleDeemedSecondary, EP._16Plus, EP.AllThrough } },
                { ET.VoluntaryControlledSchool, new[] { EP.Primary, EP.MiddleDeemedPrimary, EP.Secondary, EP.MiddleDeemedSecondary, EP._16Plus, EP.AllThrough } },
                { ET.FoundationSchool, new[] { EP.Primary, EP.MiddleDeemedPrimary, EP.Secondary, EP.MiddleDeemedSecondary, EP._16Plus, EP.AllThrough } },

                { ET.CityTechnologyCollege, new[] { EP.NotApplicable } },
                { ET.CommunitySpecialSchool, new[] { EP.NotApplicable } },
                { ET.NonmaintainedSpecialSchool, new[] { EP.NotApplicable } },
                { ET.OtherIndependentSpecialSchool, new[] { EP.NotApplicable } },
                { ET.OtherIndependentSchool, new[] { EP.NotApplicable } },
                { ET.FoundationSpecialSchool, new[] { EP.NotApplicable } },
                { ET.PupilReferralUnit, new[] { EP.NotApplicable } },

                { ET.LANurserySchool, new[] { EP.Nursery } },
                { ET.FurtherEducation, new[] { EP._16Plus } },

                { ET.SecureUnits, new[] { EP.NotApplicable } },
                { ET.OffshoreSchools, new[] { EP.NotApplicable } },
                { ET.ServiceChildrensEducation, new[] { EP.NotApplicable } },
                { ET.Miscellaneous, new[] { EP.NotApplicable } },

                { ET.AcademySponsorLed, new[] { EP.Primary, EP.MiddleDeemedPrimary, EP.Secondary, EP.MiddleDeemedSecondary, EP._16Plus, EP.AllThrough } },

                { ET.HigherEducationInstitutions, new[] { EP.NotApplicable } },
                { ET.WelshEstablishment, new[] { EP.NotApplicable } },
                { ET.SixthFormCentres, new[] { EP.NotApplicable } },
                { ET.SpecialPost16Institution, new[] { EP.NotApplicable } },
                { ET.AcademySpecialSponsorLed, new[] { EP.NotApplicable } },

                { ET.AcademyConverter, new[] { EP.Nursery, EP.Primary, EP.MiddleDeemedPrimary, EP.Secondary, EP.MiddleDeemedSecondary, EP._16Plus, EP.AllThrough } },
                { ET.FreeSchools, new[] { EP.Primary, EP.MiddleDeemedPrimary, EP.Secondary, EP.MiddleDeemedSecondary, EP.AllThrough } },

                { ET.FreeSchoolsSpecial, new[] { EP.NotApplicable } },
                { ET.BritishSchoolsOverseas, new[] { EP.NotApplicable } },
                { ET.FreeSchoolsAlternativeProvision, new[] { EP.NotApplicable } },

                { ET.FreeSchools1619, new[] { EP._16Plus } },
                { ET.UniversityTechnicalCollege, new[] { EP.Primary, EP.MiddleDeemedPrimary, EP.Secondary, EP.MiddleDeemedSecondary, EP._16Plus, EP.AllThrough } },
                { ET.StudioSchools, new[] { EP.Primary, EP.MiddleDeemedPrimary, EP.Secondary, EP.MiddleDeemedSecondary, EP._16Plus, EP.AllThrough } },

                { ET.AcademyAlternativeProvisionConverter, new[] { EP.NotApplicable } },
                { ET.AcademyAlternativeProvisionSponsorLed, new[] { EP.NotApplicable } },
                { ET.AcademySpecialConverter, new[] { EP.NotApplicable } },

                { ET.Academy1619Converter, new[] { EP._16Plus } },
                { ET.Academy1619SponsorLed, new[] { EP._16Plus } },

                { ET.ChildrensCentre, new[] { EP.NotApplicable } },
                { ET.ChildrensCentreLinkedSite, new[] { EP.NotApplicable } },
                { ET.InstitutionFundedByOtherGovernmentDepartment, new[] { EP.NotApplicable } },

                {ET.OnlineProvider, new []{ EP.Primary, EP.MiddleDeemedPrimary, EP.Secondary, EP.MiddleDeemedSecondary, EP._16Plus, EP.AllThrough }},

                {ET.AcademySecure16to19, new []{ EP._16Plus } }
            };

            return retVal;
        }

        public async Task<IEnumerable<LinkedEstablishmentModel>> GetLinkedEstablishmentsAsync(int urn, IPrincipal principal)
                            => (await _httpClient.GetAsync<List<LinkedEstablishmentModel>>($"establishment/{urn}/linked-establishments", principal)).GetResponse();

        public async Task<List<ChangeDescriptorDto>> GetModelChangesAsync(EstablishmentModel model, EstablishmentApprovalsPolicy approvalsPolicy, IPrincipal principal)
        {
            var originalModel = (await GetAsync(model.Urn.Value, principal)).GetResult();
            return await GetModelChangesAsync(originalModel, model, approvalsPolicy);
        }

        public async Task<List<ChangeDescriptorDto>> GetModelChangesAsync(EstablishmentModel original, EstablishmentModel model, EstablishmentApprovalsPolicy approvalsPolicy)
        {
            var changes = ReflectionHelper.DetectChanges(model, original, new[]{ typeof(IEBTModel), typeof(ProprietorModel)});
            changes.AddRange(await DetectAdditionalAddressChanges(original, model));
            changes.AddRange(await DetectProprietorsChanges(original, model));
            var retVal = new List<ChangeDescriptorDto>();

            var approvalFields = approvalsPolicy.GetFieldsRequiringApproval();

            foreach (var change in changes)
            {
                if (_cachedLookupService.IsLookupField(change.Name))
                {
                    change.OldValue = await _cachedLookupService.GetNameAsync(change.Name, change.OldValue.ToInteger());
                    change.NewValue = await _cachedLookupService.GetNameAsync(change.Name, change.NewValue.ToInteger());
                }

                if(change.DisplayName == null)
                {
                    change.DisplayName = PropertyName2Label(change.Name);
                }

                retVal.Add(new ChangeDescriptorDto
                {
                    Id = change.Name,
                    Name = change.DisplayName ?? change.Name,
                    NewValue = change.NewValue.Clean(),
                    OldValue = change.OldValue.Clean(),
                    Tag = change.Tag,
                    RequiresApproval = (change.Tag == "additionaladdress" && approvalsPolicy.AdditionalAddresses.RequiresApproval) ||
                                       approvalFields.Contains(change.Name, StringComparer.OrdinalIgnoreCase),
                    ApproverName = approvalsPolicy.GetApproverName(change.Name)
                });
            }

            await DetectSENChanges(original, model, approvalsPolicy, retVal);

            return retVal;
        }

        public async Task<IEnumerable<LookupDto>> GetPermissibleLocalGovernorsAsync(int urn, IPrincipal principal) => (await _httpClient.GetAsync<List<LookupDto>>($"establishment/{urn}/permissible-local-governors", principal)).GetResponse();

        public async Task<int[]> GetPermittedStatusIdsAsync(IPrincipal principal)
                            => (await _httpClient.GetAsync<List<LookupDto>>("establishment/permittedstatuses", principal)).GetResponse().Select(x => x.Id).ToArray();

        public async Task<ApiPagedResult<EstablishmentSearchResultModel>> SearchAsync(EstablishmentSearchPayload payload, IPrincipal principal)
            => (await _httpClient.PostAsync<ApiPagedResult<EstablishmentSearchResultModel>>("establishment/search", payload, principal)).GetResponse();

        public async Task<IEnumerable<EstablishmentSuggestionItem>> SuggestAsync(string text, IPrincipal principal, int take = 10)
        {
            if (text.Clean() == null)
            {
                return Enumerable.Empty<EstablishmentSuggestionItem>();
            }

            return (await _httpClient.GetAsync<List<EstablishmentSuggestionItem>>($"{ApiSuggestPath}?q={text}&take={take}", principal)).GetResponse();
        }

        private async Task<IEnumerable<ChangeDescriptor>> DetectProprietorsChanges(EstablishmentModel originalModel, EstablishmentModel newModel)
        {
            var retVal = new List<ChangeDescriptor>();
            var newProprietors  = newModel.IEBTModel.Proprietors?.Where(x => !x.Id.HasValue).ToArray();
            var editedProprietors = newModel.IEBTModel.Proprietors?.Where(x => x.Id.HasValue).ToArray();
            var removedProprietors = originalModel.IEBTModel.Proprietors?.Where(x => !newModel.IEBTModel.Proprietors.Select(y => y.Id).Contains(x.Id)).ToArray();
            var namePrefix = $"{nameof(IEBTModel)}.{nameof(IEBTModel.Proprietors)}.";


            Func<int, string, string> f = (i, fieldName) => $"{fieldName} ({i + 1})";

            if (newProprietors != null)
            {
                for (var i = 0; i < newProprietors.Length; i++)
                {
                    var index = newModel.IEBTModel.Proprietors.ToList().IndexOf(newProprietors[i]) + 1 + removedProprietors?.Length;
                    var newProprietor = newProprietors[i];
                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(newProprietor.Name),
                        DisplayName = $"Name ({index} - new)",
                        NewValue = newProprietor.Name,
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(newProprietor.Street),
                        DisplayName = $"Street ({index} - new)",
                        NewValue = newProprietor.Street
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(newProprietor.Locality),
                        DisplayName = $"Locality ({index} - new)",
                        NewValue = newProprietor.Locality
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(newProprietor.Address3),
                        DisplayName = $"Address 3 ({index} - new)",
                        NewValue = newProprietor.Address3
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(newProprietor.Town),
                        DisplayName = $"Town ({index} - new)",
                        NewValue = newProprietor.Town
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(newProprietor.CountyId),
                        DisplayName = $"County ({index} - new)",
                        NewValue = await _cachedLookupService.GetNameAsync("CountyId", newProprietor.CountyId)
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(newProprietor.Postcode),
                        DisplayName = $"Postcode ({index} - new)",
                        NewValue = newProprietor.Postcode
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(newProprietor.TelephoneNumber),
                        DisplayName = $"Telephone number ({index} - new)",
                        NewValue = newProprietor.TelephoneNumber
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(newProprietor.Email),
                        DisplayName = $"Email ({index} - new)",
                        NewValue = newProprietor.Email
                    });
                }
            }

            if (editedProprietors != null)
            {
                for (var i = 0; i < editedProprietors.Length; i++)
                {
                    var index = newModel.IEBTModel.Proprietors.ToList().IndexOf(editedProprietors[i]);
                    var proprietor = editedProprietors[i];
                    var oldProprietor = originalModel.IEBTModel.Proprietors.FirstOrDefault(x => x.Id == proprietor.Id);
                    if (oldProprietor != null)
                    {
                        var changes = ReflectionHelper.DetectChanges(proprietor, oldProprietor).AsEnumerable();
                        changes.ForEach(x => x.DisplayName = f(index, PropertyName2Label(x.Name))); // alter the field name so it contains the index of the current proprietors model
                        retVal.AddRange(changes);
                    }
                }
            }

            if (removedProprietors != null)
            {
                for (var i = 0; i < removedProprietors.Length; i++)
                {
                    var index = originalModel.IEBTModel.Proprietors?.ToList().IndexOf(removedProprietors[i]) + 1;
                    var proprietor = removedProprietors[i];
                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(proprietor.Name),
                        DisplayName = $"Name ({index} - deleting)",
                        OldValue = proprietor.Name
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(proprietor.Street),
                        DisplayName = $"Street ({index} - deleting)",
                        OldValue = proprietor.Street
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(proprietor.Locality),
                        DisplayName = $"Locality ({index} - deleting)",
                        OldValue = proprietor.Locality
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(proprietor.Address3),
                        DisplayName = $"Address 3 ({index} - deleting)",
                        OldValue = proprietor.Address3
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(proprietor.Town),
                        DisplayName = $"Town ({index} - deleting)",
                        OldValue = proprietor.Town
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(proprietor.CountyId),
                        DisplayName = $"County ({index} - deleting)",
                        OldValue = await _cachedLookupService.GetNameAsync("CountyId", proprietor.CountyId)
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(proprietor.Postcode),
                        DisplayName = $"Postcode ({index} - deleting)",
                        OldValue = proprietor.Postcode
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(proprietor.TelephoneNumber),
                        DisplayName = $"Telephone number ({index} - deleting)",
                        OldValue = proprietor.TelephoneNumber
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(proprietor.Email),
                        DisplayName = $"Email ({index} - deleting)",
                        OldValue = proprietor.Email
                    });
                }
            }

            retVal.ForEach(x => x.Tag = "proprietors");
            retVal.ForEach(x => x.Name = $"{namePrefix}{x.Name}");

            return retVal;
        }

        private async Task<IEnumerable<ChangeDescriptor>> DetectAdditionalAddressChanges(EstablishmentModel originalModel, EstablishmentModel newModel)
        {
            var retVal = new List<ChangeDescriptor>();
            var newAddresses = newModel.AdditionalAddresses?.Where(x => !x.Id.HasValue).ToArray();
            Func<int, string, string> f = (i, fieldName) => $"{fieldName} ({i + 1})";

            if (newAddresses != null)
            {
                for (var i = 0; i < newAddresses.Length; i++)
                {
                    var newAddress = newAddresses[i];
                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(newAddress.SiteName),
                        DisplayName = "Site name (new)",
                        NewValue = newAddress.SiteName
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(newAddress.Street),
                        DisplayName = "Street (new)",
                        NewValue = newAddress.Street
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(newAddress.Locality),
                        DisplayName = "Locality (new)",
                        NewValue = newAddress.Locality
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(newAddress.Address3),
                        DisplayName = "Address line 3 (new)",
                        NewValue = newAddress.Address3
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(newAddress.Town),
                        DisplayName = "City / town (new)",
                        NewValue = newAddress.Town
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(newAddress.CountyId),
                        DisplayName = "County (new)",
                        NewValue = await _cachedLookupService.GetNameAsync("CountyId", newAddress.CountyId)
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(newAddress.PostCode),
                        DisplayName = "Post code (new)",
                        NewValue = newAddress.PostCode
                    });
                }
            }

            var editedAddresses = newModel.AdditionalAddresses?.Where(x => x.Id.HasValue).ToArray();
            if (editedAddresses != null)
            {
                for (var i = 0; i < editedAddresses.Length; i++)
                {
                    var index = newModel.AdditionalAddresses.ToList().IndexOf(editedAddresses[i]) + 1;
                    var address = editedAddresses[i];
                    var oldAddress = originalModel.AdditionalAddresses.FirstOrDefault(x => x.Id == address.Id);
                    if (oldAddress != null)
                    {
                        var changes = ReflectionHelper.DetectChanges(address, oldAddress).AsEnumerable();
                        changes.ForEach(x => x.DisplayName = f(index, PropertyName2Label(x.Name))); // alter the field name so it contains the index of the current address model
                        retVal.AddRange(changes);
                    }
                }
            }

            var removedAddresses = originalModel.AdditionalAddresses?.Where(x => !newModel.AdditionalAddresses.Select(y => y.Id).Contains(x.Id)).ToArray();
            if (removedAddresses != null)
            {
                for (var i = 0; i < removedAddresses.Length; i++)
                {
                    var index = newModel.AdditionalAddresses.ToList().IndexOf(removedAddresses[i]) + 1;
                    var address = removedAddresses[i];
                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(address.SiteName),
                        DisplayName = "Site name (deleting)",
                        OldValue = address.SiteName
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(address.Street),
                        DisplayName = "Street (deleting)",
                        OldValue = address.Street
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(address.Locality),
                        DisplayName = "Locality (deleting)",
                        OldValue = address.Locality
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(address.Address3),
                        DisplayName = "Address line 3 (deleting)",
                        OldValue = address.Address3
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(address.Town),
                        DisplayName = "City / town (deleting)",
                        OldValue = address.Town
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(address.CountyId),
                        DisplayName = "County (deleting)",
                        OldValue = await _cachedLookupService.GetNameAsync("CountyId", address.CountyId)
                    });

                    retVal.Add(new ChangeDescriptor
                    {
                        Name = nameof(address.PostCode),
                        DisplayName = "Post code (deleting)",
                        OldValue = address.PostCode
                    });
                }
            }

            retVal.ForEach(x => x.Tag = "additionaladdress");

            return retVal;
        }

        private async Task DetectSENChanges(EstablishmentModel original, EstablishmentModel model, EstablishmentApprovalsPolicy approvalsPolicy,  List<ChangeDescriptorDto> retVal)
        {
            var originalSenIds = (original.SENIds ?? new int[0]).OrderBy(x => x);
            var newSenIds = (model.SENIds ?? new int[0]).OrderBy(x => x);
            if (!originalSenIds.SequenceEqual(newSenIds))
            {
                var sens = await _cachedLookupService.SpecialEducationNeedsGetAllAsync();
                var originalSenNames = StringUtil.SentencifyNoFormating(originalSenIds.Select(x => sens.FirstOrDefault(s => s.Id == x)?.Name).ToArray());
                var newSenNames = StringUtil.SentencifyNoFormating(newSenIds.Select(x => sens.FirstOrDefault(s => s.Id == x)?.Name).ToArray());
                var id = "SENIds";
                retVal.Add(new ChangeDescriptorDto
                {
                    Id = id,
                    Name = "Type of SEN provision",
                    NewValue = newSenNames,
                    OldValue = originalSenNames,
                    RequiresApproval = approvalsPolicy
                        .GetFieldsRequiringApproval()
                        .Contains(id, StringComparer.OrdinalIgnoreCase),

                    ApproverName = approvalsPolicy.GetApproverName(id)
                });
            }
        }

        private string PropertyName2Label(string name)
        {
            if (name.EndsWith("Id", StringComparison.Ordinal))
            {
                name = name.Substring(0, name.Length - 2);
            }

            name = name.Replace("_", "").Replace(nameof(IEBTModel) + ".", string.Empty);
            name = name.ToProperCase(true);
            return name;
        }
    }
}
