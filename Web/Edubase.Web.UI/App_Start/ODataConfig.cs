#if (QA)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;
using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;
using System.Diagnostics;

namespace Edubase.Web.UI
{
    public static class ODataConfig
    {
        public static void Register(HttpConfiguration config)
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

            builder.EntitySet<Establishment>("Establishments");
            builder.EntitySet<LookupDistrictAdministrative>("LookupAdministrativeDistricts");
            builder.EntitySet<LookupAdministrativeWard>("LookupAdministrativeWards");
            builder.EntitySet<LookupAdmissionsPolicy>("LookupAdmissionsPolicies");
            builder.EntitySet<LookupInspectorateName>("LookupInspectorateNames");
            builder.EntitySet<LookupCASWard>("LookupCASWards");
            builder.EntitySet<LookupDeliveryModel>("LookupCCDeliveryModels");
            builder.EntitySet<LookupDirectProvisionOfEarlyYears>("LookupDirectProvisionOfEarlyYears");
            builder.EntitySet<LookupCCDisadvantagedArea>("LookupCCDisadvantagedAreas");
            builder.EntitySet<LookupCCGovernance>("LookupCCGovernance");
            builder.EntitySet<LookupCCGroupLead>("LookupCCGroupLeads");
            builder.EntitySet<LookupCCOperationalHours>("LookupCCOperationalHours");
            builder.EntitySet<LookupCCPhaseType>("LookupCCPhaseTypes");
            builder.EntitySet<LookupChildcareFacilities>("LookupChildcareFacilities");
            builder.EntitySet<LookupDiocese>("LookupDioceses");
            builder.EntitySet<LookupEducationPhase>("LookupEducationPhases");
            builder.EntitySet<LookupEstablishmentType>("LookupEstablishmentTypes");
            builder.EntitySet<LookupEstablishmentTypeGroup>("LookupEstablishmentTypeGroups");
            builder.EntitySet<LookupFurtherEducationType>("LookupFurtherEducationTypes");
            builder.EntitySet<LookupGender>("LookupGenders");
            builder.EntitySet<LookupGovernmentOfficeRegion>("LookupGovernmentOfficeRegions");
            builder.EntitySet<LookupGSSLA>("LookupGSSLA");
            builder.EntitySet<LookupHeadTitle>("LookupHeadTitles");
            builder.EntitySet<LookupInspectorate>("LookupInspectorates");
            builder.EntitySet<LocalAuthority>("LocalAuthorities");
            builder.EntitySet<LookupLSOA>("LookupLSOAs");
            builder.EntitySet<LookupMSOA>("LookupMSOAs");
            builder.EntitySet<LookupParliamentaryConstituency>("LookupParliamentaryConstituencies");
            builder.EntitySet<LookupProvisionBoarding>("LookupProvisionBoarding");
            builder.EntitySet<LookupProvisionNursery>("LookupProvisionNurseries");
            builder.EntitySet<LookupProvisionOfficialSixthForm>("LookupProvisionOfficialSixthForms");
            builder.EntitySet<LookupProvisionSpecialClasses>("LookupProvisionSpecialClasses");
            builder.EntitySet<LookupPRUEBD>("LookupPRUEBDs");
            builder.EntitySet<LookupPruEducatedByOthers>("LookupPruEducatedByOthers");
            builder.EntitySet<LookupPruFulltimeProvision>("LookupPruFulltimeProvisions");
            builder.EntitySet<LookupPRUSEN>("LookupPRUSENs");
            builder.EntitySet<LookupReasonEstablishmentClosed>("LookupReasonEstablishmentClosed");
            builder.EntitySet<LookupReasonEstablishmentOpened>("LookupReasonEstablishmentOpened");
            builder.EntitySet<LookupReligiousCharacter>("LookupReligiousCharacters");
            builder.EntitySet<LookupReligiousEthos>("LookupReligiousEthos");
            builder.EntitySet<LookupSection41Approved>("LookupSection41Approved");
            builder.EntitySet<LookupSpecialEducationNeeds>("LookupSpecialEducationNeeds");
            builder.EntitySet<LookupEstablishmentStatus>("LookupEstablishmentStatuses");
            builder.EntitySet<LookupTeenageMothersProvision>("LookupTeenageMothersProvisions");
            builder.EntitySet<LookupTypeOfResourcedProvision>("LookupTypeOfResourcedProvisions");
            builder.EntitySet<LookupUrbanRural>("LookupUrbanRural");


            builder.EntitySet<Governor>("Governors");
            builder.EntitySet<LookupGovernorAppointingBody>("LookupGovernorAppointingBodies");
            builder.EntitySet<Establishment>("Establishments");
            builder.EntitySet<GroupCollection>("Groups");
            builder.EntitySet<LookupGovernorRole>("LookupGovernorRoles");


            builder.EntitySet<GroupCollection>("Groups");
            builder.EntitySet<LookupGroupType>("LookupGroupTypes");
            builder.EntitySet<LookupGroupStatus>("LookupGroupStatuses");

            config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());


            
        }
    }
}
#endif