namespace Edubase.Services.Establishments.Downloads
{
    public class EstablishmentDownloadFullFieldList : EstablishmentDownloadCoreFieldList
    {
        public EstablishmentDownloadFullFieldList()
        {
            OfstedRatingDetails = true;
            InspectorateId = true;
            ProprietorName = true;
            Capacity = true;
            Section41ApprovedId = true;
            ReasonEstablishmentOpenedId = true;
            ReasonEstablishmentClosedId = true;
            ProvisionSpecialClassesId = true;
            SENStat = true;
            SENNoStat = true;
            Contact_EmailAddress = true;
            ContactAlt_EmailAddress = true;
            HeadPreferredJobTitle = true;
            LastChangedDate = true;
            TypeOfSENProvisionList = true;
            TeenageMothersProvisionId = true;
            TeenageMothersCapacity = true;
            ChildcareFacilitiesId = true;
            PRUSENId = true;
            PRUEBDId = true;
            PlacesPRU = true;
            PruFulltimeProvisionId = true;
            PruEducatedByOthersId = true;
            TypeOfResourcedProvisionId = true;
            ResourcedProvisionOnRoll = true;
            ResourcedProvisionCapacity = true;
            SenUnitOnRoll = true;
            SenUnitCapacity = true;
            BSOInspectorateReportUrl = true;
            BSOInspectorateId = true;
            BSODateOfLastInspectionVisit = true;
            BSODateOfNextInspectionVisit = true;
            SetLocationFields(true);
        }
    }
}
