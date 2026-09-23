namespace Edubase.AcceptanceTests.Authentication
{
    public static class WebRoutes
    {
        public static string AcademicSponsor => "/Groups/Group/Create/Sponsor";
        public static string AcceptableUsePolicy => "/AcceptableUsePolicy";
        public static string Accessibility => "/Accessibility";
        public static string Amalgamation => "/Tools/MergersTool";
        public static string Approvals => "/Approvals";
        public static string BulkAcademies => "/Tools/BulkAcademies";
        public static string BulkCreateFreeSchoolMeals => "/Establishments/bulk-create-free-schools";
        public static string BulkUploadAcademies => "/Establishments/bulk-associate-estabs-to-groups";
        public static string BulkUpdateGovernor => "/Governors/BulkUpdate";
        public static string BulkUpdateEstablishment => "/Establishments/BulkUpdate";
        public static string ChangeHistory => "/ChangeHistory";
        public static string EdubaseHome => "/edubase/home.xhtml";
        public static string ToolsDownloadClosedSatsSsatsMats => "/Tools/DownloadClosedTrustsInformation";
        public static string ToolsDownloadClosedSatsSsatsMatsFile(string date) => $"/Downloads/Download/MATClosureReport?filename=SsatSatAndMatClosureReport_{date}.csv";
        public static string ConfirmEstablishmentDetails(int urn) => $"/Establishments/Establishment/Confirm/{urn}";

        // TODO: update this URI when the bug is fixed for the typo: https://dfe-ssp.visualstudio.com/s158-Get-Information-About-Schools/_workitems/edit/263305
        public static string ConfirmEstablishmentGovernanceDetails(int urn) => $"/Governors/Governance/ConfirmEstablishment/{urn}";
        public static string Cookies => "/Cookies";
        public static string Contact => "/Contact";
        public static string CreateAcademyTrust(string companiesHouseNumber) => $"/Groups/Group/CreateAcademyTrust/{companiesHouseNumber}";
        public static string CreateChildrensCentre => "/Groups/Group/Create/ChildrensCentre";
        public static string CreateEstablishment => "/Establishments/Establishment/Create";
        public static string CreateFederation => "/Groups/Group/Create/Federation";
        public static string CreateLASet => "/independent-schools/predefined-local-authority-sets/create";
        public static string CreateSecureSingleAcademyTrust => "/Groups/Group/SearchCompaniesHouse/secure-academy-trust";
        public static string DataQuality => "/DataQuality";
        public static string Downloads => "/Downloads";
        public static string DownloadsCollate => "/Downloads/Collate";
        public static string DownloadsDateFilter(int day, int month, int year) => $"/Downloads?Skip=&SearchType=DateFilter&FilterDate.Day={day}&FilterDate.Month={month}&FilterDate.Year={year}&action=";
        public static string DownloadsGenerated => "/Downloads/Generated/";
        public static string DownloadsScheduledExtracts => "/Downloads#scheduled-extracts";
        public static string DownloadsRequestScheduledExtract(int id) => $"/Downloads/RequestScheduledExtract/{id}";
        public static string EditEstablishment(int urn) => $"/Establishments/Establishment/Edit/{urn}";
        public static string EditEstablishmentAddress(int urn) => $"/Establishments/Establishment/Edit/{urn}/Address/Main";
        public static string EditEstablishmentIebt(int urn) => $"/Establishments/Establishment/Edit/{urn}/IEBT";
        public static string EstablishmentLocation(int urn) => $"/Establishments/Establishment/Details/{urn}#school-location";
        public static string EditEstablishmentLocation(int urn) => $"/Establishments/Establishment/Edit/{urn}/Location";
        public static string EditEstablishmentGovernance(int urn) => $"/Establishment/Edit/{urn}/Governance";
        public static string EditEstablishmentGovernanceMode(int urn) => $"/Establishment/Edit/{urn}/GovernanceMode";
        public static string EditGroupGovernance(int uid) => $"{Groups}/Edit/{uid}/Governance";
        public static string EditChairOfTrusteeHistoricGovernors(int uid, string gid) => $"{Groups}/Edit/{uid}/Governance/Edit/{gid}";
        public static string EstablishmentDetails(int urn) => $"/Establishments/Establishment/Details/{urn}";
        public static string EstablishmentGovernance(int urn) => $"/Establishments/Establishment/Details/{urn}#school-governance";
        public static string EstablishmentGovernanceAddPerson(int urn, string requestVerificationToken, string role)
            => $"/Establishment/Edit/{urn}/Governance/Add?__RequestVerificationToken={requestVerificationToken}&role={role}";
        public static string EstablishmentGovernanceAdd(int urn) => $"/Establishment/Edit/{urn}/Governance/Add";
        public static string EstablishmentGovernanceEditPerson(int urn, int gid) => $"/Establishment/Edit/{urn}/Governance/Edit/{gid}";
        public static string EstablishmentRemoveGovernor(int urn, int governorGid) => $"/Establishment/Edit/{urn}/Governance?removalGid={governorGid}";
        public static string EstablishmentRemoveGovernorEdit(int urn) => $"/Establishment/Edit/{urn}/Governance";
        public static string EstablishmentGroupDetails(int uid) => $"{Groups}/Details/{uid}";
        public static string EstablishmentGroupEditDetails(int uid) => $"{Groups}/Edit/{uid}/Details";
        public static string EstablishmentGroupEditGovernance(int uid) => $"{Groups}/Edit/{uid}/Governance";
        public static string EstablishmentGroupGovernance(int uid) => $"{Groups}/Details/{uid}#governance";
        public static string EstablishmentGroupGovernanceAdd(int uid) => $"{Groups}/Edit/{uid}/Governance/Add";
        public static string EstablishmentGroupGovernanceAddPerson(int uid, string requestVerificationToken, string role)
            => $"/{Groups}/Edit/{uid}/Governance/Add?__RequestVerificationToken={requestVerificationToken}&role={role}";
        public static string EstablishmentGroupGovernanceEditPerson(int uid, int gid) => $"/{Groups}/Edit/{uid}/Governance/Edit/{gid}";
        public static string EstablishmentIebt(int urn) => $"/Establishments/Establishment/Details/{urn}#school-iebt";
        public static string EstablishmentLinks(int urn) => $"/Establishments/Establishment/Details/{urn}#school-links";
        public static string EstablishmentEditLinks(int urn) => $"/Establishments/Establishment/Edit/{urn}/Links";
        public static string EstablishmentEditLinksSearch(int urn) => $"/Establishments/Establishment/Edit/{urn}/Links/Search";
        public static string EstablishmentEditLinksSearchResult(int urnToBeLinked, int urnLinkedTo) => $"/Establishments/Establishment/Edit/{urnToBeLinked}/Links/Search?SearchUrn={urnLinkedTo}&s=true";
        public static string EstablishmentEditLinksCreate(int urnToBeLinked, int urnLinkedTo) => $"/Establishments/Establishment/Edit/{urnToBeLinked}/Link/Create/{urnLinkedTo}";
        public static string EstablishmentPostCreateLink(int urn) => $"/Establishments/Establishment/Edit/{urn}/Link";
        public static string EstablishmentEditLink(int urn, string linkId) => $"/Establishments/Establishment/Edit/{urn}/Link/{linkId}";
        public static string Faq => "/Faq";
        public static string FaqCreateEntry => "/Faq/Create";
        public static string FaqCreateGroup => "/Faq/Groups/New";
        public static string FaqEditGroup(string id) => $"/Faq/Group/{id}";
        public static string FaqEditEntry(string id) => $"/Faq/Edit/{id}";
        public static string Glossary => "/Glossary";
        private static string Groups => @"/Groups/Group";
        public static string Guidance => "/Guidance";
        public static string Help => "/Help";
        public static string Helpdesk(int urn) => $"/Establishments/Establishment/Details/{urn}#helpdesk";
        public static string HelpdeskEdit(int urn) => $"/Establishments/Establishment/Edit/{urn}/Helpdesk";
        public static string Home => "/";
        public static string IndependentSchoolsActionDates => "/independent-schools";
        public static string ManageAcademyOpenings => "/Establishments/manage/academy-openings#calendar";
        public static string ManageSecureAcademyOpenings => "/Establishments/manage/academy-openings?skip=0&sortBy=OpenDate-desc&establishmentTypeId=46#calendar";
        public static string NewNewsArticle => "/News/Article/New";
        public static string NewNotificationBanner => "/Notifications/Banner/New";
        public static string PredefinedLASet => "/independent-schools/predefined-local-authority-sets";
        public static string Privacy => "/Privacy";
        public static string SignIn => "/Account/Login?returnUrl=%2F";
        public static string SignInSimulatorTest => "https://dfe-sign-in-simulator.azurewebsites.net/e00bdaf5-4cee-47c2-b76c-41b00bb59d02";
        public static string SignInSimulatorDev => "https://dfe-sign-in-simulator.azurewebsites.net/c4cdae40-d07b-469e-b505-350e07ee2e32";
        public static string Search => "/Search";
        public static string SearchEstablishment => "/Search?SelectedTab=Establishments";
        public static string SearchEstablishmentGroup => "/Search?SelectedTab=Groups";
        public static string SearchGovernor => "/Search?SelectedTab=Governors";
        public static string SearchResultsAllEstablishments => "/Establishments/Search?SelectedTab=Establishments&SearchType=EstablishmentAll&SearchType=EstablishmentAll&OpenOnly=true&TextSearchModel.AutoSuggestValue=";
        public static string SearchEstablishmentResultsDownloads => "/Establishments/Search/PrepareDownload?SelectedTab=Establishments&SearchType=Text&SearchType=Text&TextSearchModel.Text=ark&OpenOnly=true&TextSearchModel.AutoSuggestValue=&f=true&b=1&b=4";
        public static string ServiceInfo => "/Responsibilities";
        public static string TermsOfUse => "/TermsOfUse";
        public static string Tools => "/Tools";

    }
}
