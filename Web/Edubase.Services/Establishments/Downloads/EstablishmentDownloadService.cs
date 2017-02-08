using Edubase.Common.Cache;
using Edubase.Common.IO;
using Edubase.Common.Reflection;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Exceptions;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using Edubase.Services.Lookup;
using Ionic.Zip;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using Edubase.Services.Groups;
using Edubase.Services.Enums;
using Edubase.Services.Core;
using Edubase.Common;

namespace Edubase.Services.Establishments.Downloads
{
    public class EstablishmentDownloadService : FileDownloadFactoryService, IEstablishmentDownloadService
    {
        private ICacheAccessor _cacheAccessor;
        private IEstablishmentReadService _establishmentReadService;
        private ICachedLookupService _cachedLookupService;
        private IMessageLoggingService _messageLoggingService;
        private IGroupReadService _groupReadService;

        public enum eDataSet
        {
            Core,
            Full
        }

        public EstablishmentDownloadService(ICacheAccessor cacheAccessor, 
            IEstablishmentReadService establishmentReadService, 
            IBlobService blobService,
            ICachedLookupService cachedLookupService,
            IMessageLoggingService messageLoggingService,
            IGroupReadService groupReadService) 
            : base(cacheAccessor, blobService)
        {
            _cacheAccessor = cacheAccessor;
            _establishmentReadService = establishmentReadService;
            _cachedLookupService = cachedLookupService;
            _messageLoggingService = messageLoggingService;
            _groupReadService = groupReadService;
        }

        /// <summary>
        /// Creates a download
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="payload"></param>
        /// <param name="principal"></param>
        /// <param name="fieldList">e.g. EstablishmentDownloadCoreFieldList or EstablishmentDownloadFullFieldList</param>
        /// <returns></returns>
        public async Task SearchWithDownloadGenerationAsync(Guid taskId, EstablishmentSearchPayload payload, IPrincipal principal, eDataSet dataSet, eFileFormat format)
        {
            Guard.IsNotNull(principal, () => new ArgumentNullException(nameof(principal)));
            Guard.IsNotNull(payload, () => new ArgumentNullException(nameof(payload)));

            var progress = await _cacheAccessor.GetAsync<SearchDownloadGenerationProgressDto>(taskId.ToString());
            if (progress == null) throw new Exception($"Search download has not been initialised; call {nameof(SearchWithDownloadGeneration_InitialiseAsync)} first");

            var fieldList = dataSet == eDataSet.Core ? new EstablishmentDownloadCoreFieldList() : new EstablishmentDownloadFullFieldList();
            Func<Task> updateProgressCache = async () => await _cacheAccessor.SetAsync(taskId.ToString(), progress, TimeSpan.FromHours(12));
            progress.Status = "Initialising...";
            await updateProgressCache();

            try
            {
                payload.Skip = 0;
                payload.Take = 1000;
                var results = await _establishmentReadService.SearchAsync(payload, principal);
                progress.TotalRecordsCount = results.Count.Value;
                progress.Status = "Retrieving data...";
                progress.FileExtension = ToFileExtension(format);
                await updateProgressCache();

                var tempPath = DirectoryHelper.CreateTempDirectory().FullName;
                var fileName = Path.Combine(tempPath, string.Concat("edubase-search-results", progress.FileExtension));

                if (format == eFileFormat.CSV)
                {
                    await GenerateCsvFile(payload, principal, progress, fieldList, 
                        updateProgressCache, results, fileName);
                }
                else
                {
                    await GenerateXlsxFile(payload, principal, progress, fieldList,
                        updateProgressCache, results, fileName);
                }

                progress.Status = "Creating zip file...";
                await updateProgressCache();
                string zipFileName = CreateZipFile(fileName);

                progress.Status = "Preparing download package...";
                await updateProgressCache();
                string url = await UploadFileToBlobStorage(zipFileName);

                progress.FileLocation = url;
                progress.IsComplete = true;
                await updateProgressCache();

                Directory.Delete(tempPath, true);
                File.Delete(zipFileName);
            }
            catch (Exception ex)
            {
                progress.ExceptionMessageId = _messageLoggingService.Push(ex);
                await updateProgressCache();
            }
        }

        private async Task GenerateXlsxFile(EstablishmentSearchPayload payload,
            IPrincipal principal,
            SearchDownloadGenerationProgressDto progress,
            EstablishmentDownloadCoreFieldList fieldList,
            Func<Task> updateProgressCache,
            AzureSearchResult<SearchEstablishmentDocument> results,
            string fileName)
        {
            var headers = GetHeaders(fieldList);

            using (var xlsx = new ExcelPackage(new FileInfo(fileName)))
            {
                xlsx.Workbook.Properties.Author = "Department for Education";
                xlsx.Workbook.Properties.Title = "Edubase Search Results";

                var cursor = 1;
                var sheet = xlsx.Workbook.Worksheets.Add("Search results");
                headers.ForEach((header, index) =>
                {
                    var cell = sheet.Cells[cursor, (index + 1)];
                    cell.Value = header;
                    cell.Style.Font.Bold = true;
                });
                cursor++;

                while (results.Items.Any())
                {
                    foreach (var item in results.Items)
                    {
                        var data = await GetRowData(item, principal, fieldList);
                        data.ForEach((columnValue, index) => sheet.Cells[cursor, (index + 1)].Value = columnValue);
                        progress.ProcessedCount++;
                        cursor++;
                    }

                    payload.Skip += 1000;
                    results = await _establishmentReadService.SearchAsync(payload, principal);
                    await updateProgressCache();
                }

                xlsx.Save();
            }
        }

        private async Task GenerateCsvFile(EstablishmentSearchPayload payload, 
            IPrincipal principal, 
            SearchDownloadGenerationProgressDto progress, 
            EstablishmentDownloadCoreFieldList fieldList, 
            Func<Task> updateProgressCache, 
            AzureSearchResult<SearchEstablishmentDocument> results, 
            string fileName)
        {
            var headers = GetHeaders(fieldList);

            using (var fs = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                using (var streamWriter = new StreamWriter(fs))
                {
                    await streamWriter.WriteLineAsync(ToCsv(headers));
                    while (results.Items.Any())
                    {
                        foreach (var item in results.Items)
                        {
                            var csv = ToCsv(await GetRowData(item, principal, fieldList));
                            await streamWriter.WriteLineAsync(csv);
                            progress.ProcessedCount++;
                        }

                        payload.Skip += 1000;
                        results = await _establishmentReadService.SearchAsync(payload, principal);
                        await updateProgressCache();
                    }
                }
            }
        }


        private List<string> GetHeaders(EstablishmentFieldList fieldList)
        {
            var headers = new List<string>();
            if (fieldList.Name) headers.Add("Establishment name");
            if (fieldList.Address_Line1) headers.Add("Street");
            if (fieldList.Address_Locality) headers.Add("Locality");
            if (fieldList.Address_Line3) headers.Add("Address 3");
            if (fieldList.Address_CityOrTown) headers.Add("City/Town");
            if (fieldList.Address_County) headers.Add("County");
            if (fieldList.Address_PostCode) headers.Add("Post code");
            if (fieldList.LocalAuthorityId) headers.Add("Local authority name");
            if (fieldList.HeadTitleId) headers.Add("Head/manager title");
            if (fieldList.HeadFirstName) headers.Add("Head/manager first name");
            if (fieldList.HeadLastName) headers.Add("Head/manager surname");
            if (fieldList.StatutoryLowAge) headers.Add("Statutory low age");
            if (fieldList.StatutoryHighAge) headers.Add("Statutory high age");
            if (fieldList.EducationPhaseId) headers.Add("Phase of education");
            if (fieldList.TypeId) headers.Add("Type of establishment");
            if (fieldList.FurtherEducationTypeId) headers.Add("Further education type");

           
            if (fieldList.GroupDetails)
            {
                headers.Add("Single Academy trust");
                headers.Add("Multi academy trust");
                headers.Add("Academy sponsor");
                headers.Add("Academy co-sponsor");
                headers.Add("Trust");
                headers.Add("Federation");
            }

            if (fieldList.GenderId) headers.Add("Gender");
            if (fieldList.Urn) headers.Add("URN");
            if (fieldList.LocalAuthorityId) headers.Add("LA code");
            if (fieldList.EstablishmentNumber) headers.Add("Establishment number");
            if (fieldList.UKPRN) headers.Add("UKPRN");
            if (fieldList.StatusId) headers.Add("Status");
            if (fieldList.AdmissionsPolicyId) headers.Add("Admissions policy");
            if (fieldList.Contact_WebsiteAddress) headers.Add("Website");
            if (fieldList.Contact_TelephoneNumber) headers.Add("Telephone");
            if (fieldList.OfstedRating) headers.Add("Ofsted rating");
            if (fieldList.OfstedInspectionDate) headers.Add("Ofsted last inspection");
            if (fieldList.OfstedRating) headers.Add("Osted report link");
            if (fieldList.InspectorateId) headers.Add("Ofsted Inspectorate");

            if (fieldList.ProprietorName) headers.Add("Proprietor's name");
            if (fieldList.ReligiousCharacterId) headers.Add("Religious character");
            if (fieldList.DioceseId) headers.Add("Diocese");
            if (fieldList.ReligiousEthosId) headers.Add("Religious ethos");
            if (fieldList.ProvisionBoardingId) headers.Add("Boarders");
            if (fieldList.ProvisionNurseryId) headers.Add("Nursery provision");
            if (fieldList.ProvisionOfficialSixthFormId) headers.Add("Official sixth form");
            if (fieldList.Capacity) headers.Add("Capacity");
            if (fieldList.Section41ApprovedId) headers.Add("Section 41 approved");
            if (fieldList.OpenDate) headers.Add("Open date");
            if (fieldList.ReasonEstablishmentOpenedId) headers.Add("Reason establishment opened");
            if (fieldList.CloseDate) headers.Add("Close date");
            if (fieldList.ReasonEstablishmentClosedId) headers.Add("Reason establishment closed");
            if (fieldList.ProvisionSpecialClassesId) headers.Add("Special classes");
            if (fieldList.SENStat) headers.Add("Number of Special Pupils Under a SEN Statement/EHCP");
            if (fieldList.SENNoStat) headers.Add("Number of Special Pupils Not Under a SEN Statement/EHCP");
            if (fieldList.Contact_EmailAddress) headers.Add("Main email");
            if (fieldList.ContactAlt_EmailAddress) headers.Add("Alternative email");
            if (fieldList.HeadPreferredJobTitle) headers.Add("Head/manager preferred job title");
            if (fieldList.LastChangedDate) headers.Add("Last changed date");
            if (fieldList.TypeOfSENProvisionList) headers.Add("Type of SEN provision");
            if (fieldList.TeenageMothersProvisionId) headers.Add("PRU: Teenage Mothers");
            if (fieldList.TeenageMothersCapacity) headers.Add("PRU: Teenage Mothers Places");
            if (fieldList.ChildcareFacilitiesId) headers.Add("PRU: Childcare Facilities");
            if (fieldList.PRUSENId) headers.Add("PRU: SEN Facilities");
            if (fieldList.PRUEBDId) headers.Add("PRU: Pupils With EBD");
            if (fieldList.PlacesPRU) headers.Add("PRU: Number of Places");
            if (fieldList.PruFulltimeProvisionId) headers.Add("PRU: Full Time Provision");
            if (fieldList.PruEducatedByOthersId) headers.Add("PRU: Pupils Educated By Other providers");
            if (fieldList.TypeOfResourcedProvisionId) headers.Add("Type of resourced provision");
            if (fieldList.ResourcedProvisionOnRoll) headers.Add("Resourced provision number on roll");
            if (fieldList.ResourcedProvisionCapacity) headers.Add("Resourced provision capacity");
            if (fieldList.SenUnitOnRoll) headers.Add("SEN Unit number on roll");
            if (fieldList.SenUnitCapacity) headers.Add("SEN Unit capacity");
            if (fieldList.BSOInspectorateId) headers.Add("BSO: Inspectorate Name");
            if (fieldList.BSOInspectorateReportUrl) headers.Add("BSO: Inspectorate Report");
            if (fieldList.BSODateOfLastInspectionVisit) headers.Add("BSO: Date of Last Inspection Visit");
            if (fieldList.BSODateOfNextInspectionVisit) headers.Add("BSO: Next Inspection Visit");
            if (fieldList.RSCRegionId) headers.Add("RSC Region");
            if (fieldList.GovernmentOfficeRegionId) headers.Add("Government Office Region");
            if (fieldList.AdministrativeDistrictId) headers.Add("District");
            if (fieldList.AdministrativeWardId) headers.Add("Ward");
            if (fieldList.ParliamentaryConstituencyId) headers.Add("Parliamentary Constituency");
            if (fieldList.UrbanRuralId) headers.Add("Urban/Rural Description");
            if (fieldList.GSSLAId) headers.Add("GSS LA Code");
            if (fieldList.Easting) headers.Add("Easting");
            if (fieldList.Northing) headers.Add("Northing");
            if (fieldList.CASWardId) headers.Add("Census Ward");
            if (fieldList.MSOAId) headers.Add("Middle Super Output Area (MSOA)");
            if (fieldList.LSOAId) headers.Add("Lower Super Output Area (LSOA)");
            return headers;
        }

        private async Task<List<string>> GetRowData(SearchEstablishmentDocument doc, IPrincipal principal, EstablishmentFieldList list)
        {   
            var lsvc = _cachedLookupService;
            var dp = _establishmentReadService.GetDisplayPolicy(principal, doc);
            var fields = new List<string>();

            Action<bool, bool, object> AddIf = (shouldIncludeField, isValueAllowedByPolicy, theValue) =>
            {
                if(theValue?.GetType() == typeof(DateTime)) theValue = ((DateTime)theValue).ToString("dd/MM/yyyy");
                if(shouldIncludeField) fields.Add(isValueAllowedByPolicy ? (theValue?.ToString() ?? string.Empty) : string.Empty);
            };

            Func<bool, bool, Func<Task<string>>, Task> AddIfAsync = async (shouldIncludeField, isValueAllowedByPolicy, factory) =>
            {
                if (shouldIncludeField) fields.Add(isValueAllowedByPolicy ? (await factory()) ?? string.Empty : string.Empty);
            };

            AddIf(list.Name, dp.Name, doc.Name);
            AddIf(list.Address_Line1, dp.Address_Line1, doc.Address_Line1);
            AddIf(list.Address_Locality, dp.Address_Locality, doc.Address_Locality);
            AddIf(list.Address_Line3, dp.Address_Line3, doc.Address_Line3);
            AddIf(list.Address_CityOrTown, dp.Address_CityOrTown, doc.Address_CityOrTown);
            AddIf(list.Address_County, dp.Address_County, doc.Address_County);
            AddIf(list.Address_PostCode, dp.Address_PostCode, doc.Address_PostCode);
            
            await AddIfAsync(list.LocalAuthorityId, dp.LocalAuthorityId, async () => await lsvc.GetNameAsync(nameof(doc.LocalAuthorityId), doc.LocalAuthorityId));
            await AddIfAsync(list.HeadTitleId, dp.HeadTitleId, async () => await lsvc.GetNameAsync(nameof(doc.HeadTitleId), doc.HeadTitleId));

            AddIf(list.HeadFirstName, dp.HeadFirstName, doc.HeadFirstName);
            AddIf(list.HeadLastName, dp.HeadLastName, doc.HeadLastName);
            AddIf(list.StatutoryLowAge, dp.StatutoryLowAge, doc.StatutoryLowAge);
            AddIf(list.StatutoryHighAge, dp.StatutoryHighAge, doc.StatutoryHighAge);
            await AddIfAsync(list.EducationPhaseId, dp.EducationPhaseId, async () => await lsvc.GetNameAsync(nameof(doc.EducationPhaseId), doc.EducationPhaseId));
            await AddIfAsync(list.TypeId, dp.TypeId, async () => await lsvc.GetNameAsync(nameof(doc.TypeId), doc.TypeId));
            await AddIfAsync(list.FurtherEducationTypeId, dp.FurtherEducationTypeId, async () => await lsvc.GetNameAsync(nameof(doc.FurtherEducationTypeId), doc.FurtherEducationTypeId));


            if (list.GroupDetails)
            {
                var groups = await _groupReadService.GetAllByEstablishmentUrnAsync(doc.Urn.Value);
                AddIf(true, dp.GroupDetails, groups.FirstOrDefault(x => x.GroupTypeId == (int)eLookupGroupType.SingleacademyTrust)?.Name);
                AddIf(true, dp.GroupDetails, groups.FirstOrDefault(x => x.GroupTypeId == (int)eLookupGroupType.MultiacademyTrust)?.Name);
                AddIf(true, dp.GroupDetails, groups.FirstOrDefault(x => x.GroupTypeId == (int)eLookupGroupType.SchoolSponsor)?.Name);
                AddIf(true, dp.GroupDetails, ""); // todo: Academy co-sponsor!!
                AddIf(true, dp.GroupDetails, groups.FirstOrDefault(x => x.GroupTypeId == (int)eLookupGroupType.Trust)?.Name);
                AddIf(true, dp.GroupDetails, groups.FirstOrDefault(x => x.GroupTypeId == (int)eLookupGroupType.Federation)?.Name);
            }

            await AddIfAsync(list.GenderId, dp.GenderId, async () => await lsvc.GetNameAsync(nameof(doc.GenderId), doc.GenderId));

            AddIf(list.Urn, dp.Urn, doc.Urn);
            AddIf(list.LocalAuthorityId, dp.LocalAuthorityId, doc.LocalAuthorityId);
            AddIf(list.EstablishmentNumber, dp.EstablishmentNumber, doc.EstablishmentNumber);
            AddIf(list.UKPRN, dp.UKPRN, doc.UKPRN);
            await AddIfAsync(list.StatusId, dp.StatusId, async () => await lsvc.GetNameAsync(nameof(doc.StatusId), doc.StatusId));
            await AddIfAsync(list.AdmissionsPolicyId, dp.AdmissionsPolicyId, async () => await lsvc.GetNameAsync(nameof(doc.AdmissionsPolicyId), doc.AdmissionsPolicyId));

            AddIf(list.Contact_WebsiteAddress, dp.Contact_WebsiteAddress, doc.Contact_WebsiteAddress);
            AddIf(list.Contact_TelephoneNumber, dp.Contact_TelephoneNumber, doc.Contact_TelephoneNumber);
            AddIf(list.OfstedRating, dp.OfstedRating, doc.OfstedRating);
            AddIf(list.OfstedInspectionDate, dp.OfstedInspectionDate, doc.OfstedInspectionDate);
            AddIf(list.OfstedRating, dp.OfstedRating, new OfstedRatingUrl(doc.Urn));

            await AddIfAsync(list.InspectorateId, dp.InspectorateId, async () => await lsvc.GetNameAsync(nameof(doc.InspectorateId), doc.InspectorateId));

            AddIf(list.ProprietorName, dp.ProprietorName, doc.ProprietorName);
            await AddIfAsync(list.ReligiousCharacterId, dp.ReligiousCharacterId, async () => await lsvc.GetNameAsync(nameof(doc.ReligiousCharacterId), doc.ReligiousCharacterId));
            await AddIfAsync(list.DioceseId, dp.DioceseId, async () => await lsvc.GetNameAsync(nameof(doc.DioceseId), doc.DioceseId));
            await AddIfAsync(list.ReligiousEthosId, dp.ReligiousEthosId, async () => await lsvc.GetNameAsync(nameof(doc.ReligiousEthosId), doc.ReligiousEthosId));
            await AddIfAsync(list.ProvisionBoardingId, dp.ProvisionBoardingId, async () => await lsvc.GetNameAsync(nameof(doc.ProvisionBoardingId), doc.ProvisionBoardingId));
            await AddIfAsync(list.ProvisionNurseryId, dp.ProvisionNurseryId, async () => await lsvc.GetNameAsync(nameof(doc.ProvisionNurseryId), doc.ProvisionNurseryId));
            await AddIfAsync(list.ProvisionOfficialSixthFormId, dp.ProvisionOfficialSixthFormId, async () => await lsvc.GetNameAsync(nameof(doc.ProvisionOfficialSixthFormId), doc.ProvisionOfficialSixthFormId));

            AddIf(list.Capacity, dp.Capacity, doc.Capacity);
            await AddIfAsync(list.Section41ApprovedId, dp.Section41ApprovedId, async () => await lsvc.GetNameAsync(nameof(doc.Section41ApprovedId), doc.Section41ApprovedId));

            AddIf(list.OpenDate, dp.OpenDate, doc.OpenDate); 
            await AddIfAsync(list.ReasonEstablishmentOpenedId, dp.ReasonEstablishmentOpenedId, async () => await lsvc.GetNameAsync(nameof(doc.ReasonEstablishmentOpenedId), doc.ReasonEstablishmentOpenedId));
            AddIf(list.CloseDate, dp.CloseDate, doc.CloseDate);
            await AddIfAsync(list.ReasonEstablishmentClosedId, dp.ReasonEstablishmentClosedId, async () => await lsvc.GetNameAsync(nameof(doc.ReasonEstablishmentClosedId), doc.ReasonEstablishmentClosedId));
            await AddIfAsync(list.ProvisionSpecialClassesId, dp.ProvisionSpecialClassesId, async () => await lsvc.GetNameAsync(nameof(doc.ProvisionSpecialClassesId), doc.ProvisionSpecialClassesId));
            AddIf(list.SENStat, dp.SENStat, doc.SENStat);
            AddIf(list.SENNoStat, dp.SENNoStat, doc.SENNoStat);
            AddIf(list.Contact_EmailAddress, dp.Contact_EmailAddress, doc.Contact_EmailAddress);
            AddIf(list.ContactAlt_EmailAddress, dp.ContactAlt_EmailAddress, doc.ContactAlt_EmailAddress);
            AddIf(list.HeadPreferredJobTitle, dp.HeadPreferredJobTitle, doc.HeadPreferredJobTitle);
            AddIf(list.LastChangedDate, dp.LastChangedDate, doc.LastChangedDate); 
            AddIf(list.TypeOfSENProvisionList, dp.TypeOfSENProvisionList, await GetTypeOfSENProvisionList(doc));
            await AddIfAsync(list.TeenageMothersProvisionId, dp.TeenageMothersProvisionId, async () => await lsvc.GetNameAsync(nameof(doc.TeenageMothersProvisionId), doc.TeenageMothersProvisionId));
            AddIf(list.TeenageMothersCapacity, dp.TeenageMothersCapacity, doc.TeenageMothersCapacity);
            await AddIfAsync(list.ChildcareFacilitiesId, dp.ChildcareFacilitiesId, async () => await lsvc.GetNameAsync(nameof(doc.ChildcareFacilitiesId), doc.ChildcareFacilitiesId));
            await AddIfAsync(list.PRUSENId, dp.PRUSENId, async () => await lsvc.GetNameAsync(nameof(doc.PRUSENId), doc.PRUSENId));
            await AddIfAsync(list.PRUEBDId, dp.PRUEBDId, async () => await lsvc.GetNameAsync(nameof(doc.PRUEBDId), doc.PRUEBDId));
            AddIf(list.PlacesPRU, dp.PlacesPRU, doc.PlacesPRU);
            await AddIfAsync(list.PruFulltimeProvisionId, dp.PruFulltimeProvisionId, async () => await lsvc.GetNameAsync(nameof(doc.PruFulltimeProvisionId), doc.PruFulltimeProvisionId));
            await AddIfAsync(list.PruEducatedByOthersId, dp.PruEducatedByOthersId, async () => await lsvc.GetNameAsync(nameof(doc.PruEducatedByOthersId), doc.PruEducatedByOthersId));
            await AddIfAsync(list.TypeOfResourcedProvisionId, dp.TypeOfResourcedProvisionId, async () => await lsvc.GetNameAsync(nameof(doc.TypeOfResourcedProvisionId), doc.TypeOfResourcedProvisionId));
            AddIf(list.ResourcedProvisionOnRoll, dp.ResourcedProvisionOnRoll, doc.ResourcedProvisionOnRoll);
            AddIf(list.ResourcedProvisionCapacity, dp.ResourcedProvisionCapacity, doc.ResourcedProvisionCapacity);
            AddIf(list.SenUnitOnRoll, dp.SenUnitOnRoll, doc.SenUnitOnRoll);
            AddIf(list.SenUnitCapacity, dp.SenUnitCapacity, doc.SenUnitCapacity);
            await AddIfAsync(list.BSOInspectorateId, dp.BSOInspectorateId, async () => await lsvc.GetNameAsync(nameof(doc.BSOInspectorateId), doc.BSOInspectorateId));
            AddIf(list.BSOInspectorateReportUrl, dp.BSOInspectorateReportUrl, doc.BSOInspectorateReportUrl);
            AddIf(list.BSODateOfLastInspectionVisit, dp.BSODateOfLastInspectionVisit, doc.BSODateOfLastInspectionVisit);
            AddIf(list.BSODateOfNextInspectionVisit, dp.BSODateOfNextInspectionVisit, doc.BSODateOfNextInspectionVisit);

            await AddIfAsync(list.RSCRegionId, dp.RSCRegionId, async () => await lsvc.GetNameAsync(nameof(doc.RSCRegionId), doc.RSCRegionId));
            await AddIfAsync(list.GovernmentOfficeRegionId, dp.GovernmentOfficeRegionId, async () => await lsvc.GetNameAsync(nameof(doc.GovernmentOfficeRegionId), doc.GovernmentOfficeRegionId));
            await AddIfAsync(list.AdministrativeDistrictId, dp.AdministrativeDistrictId, async () => await lsvc.GetNameAsync(nameof(doc.AdministrativeDistrictId), doc.AdministrativeDistrictId));
            await AddIfAsync(list.AdministrativeWardId, dp.AdministrativeWardId, async () => await lsvc.GetNameAsync(nameof(doc.AdministrativeWardId), doc.AdministrativeWardId));
            await AddIfAsync(list.ParliamentaryConstituencyId, dp.ParliamentaryConstituencyId, async () => await lsvc.GetNameAsync(nameof(doc.ParliamentaryConstituencyId), doc.ParliamentaryConstituencyId));
            await AddIfAsync(list.UrbanRuralId, dp.UrbanRuralId, async () => await lsvc.GetNameAsync(nameof(doc.UrbanRuralId), doc.UrbanRuralId));
            await AddIfAsync(list.GSSLAId, dp.GSSLAId, async () => await lsvc.GetNameAsync(nameof(doc.GSSLAId), doc.GSSLAId));

            AddIf(list.Easting, dp.Easting, doc.Easting);
            AddIf(list.Northing, dp.Northing, doc.Northing);

            await AddIfAsync(list.CASWardId, dp.CASWardId, async () => await lsvc.GetNameAsync(nameof(doc.CASWardId), doc.CASWardId));
            await AddIfAsync(list.MSOAId, dp.MSOAId, async () => await lsvc.GetNameAsync(nameof(doc.MSOAId), doc.MSOAId));
            await AddIfAsync(list.LSOAId, dp.LSOAId, async () => await lsvc.GetNameAsync(nameof(doc.LSOAId), doc.LSOAId));

            return fields;
        }

        private async Task<string> GetTypeOfSENProvisionList(SearchEstablishmentDocument doc)
        {
            var sens = new List<string>();
            var sen = await _cachedLookupService.GetNameAsync(nameof(doc.SEN1Id), doc.SEN1Id);
            if (sen != null) sens.Add(sen);

            sen = await _cachedLookupService.GetNameAsync(nameof(doc.SEN2Id), doc.SEN2Id);
            if (sen != null) sens.Add(sen);

            sen = await _cachedLookupService.GetNameAsync(nameof(doc.SEN3Id), doc.SEN3Id);
            if (sen != null) sens.Add(sen);

            sen = await _cachedLookupService.GetNameAsync(nameof(doc.SEN4Id), doc.SEN4Id);
            if (sen != null) sens.Add(sen);

            return string.Join(", ", sens);
        }
        
    }
}
