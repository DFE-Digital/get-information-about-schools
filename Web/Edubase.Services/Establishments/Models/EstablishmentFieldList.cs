namespace Edubase.Services.Establishments.Models
{
    /// <summary>
    /// Represents the list of fields within an Establishment.
    /// </summary>
    public abstract class EstablishmentFieldList : EstablishmentFieldListBase
    {
        public bool HeadteacherDetails
        {
            set { HeadFirstName = HeadLastName = HeadTitleId = value; }
            get { return HeadFirstName && HeadLastName && HeadTitleId; }
        }

        public bool AgeRange
        {
            set { StatutoryHighAge = StatutoryLowAge = value; }
            get { return StatutoryHighAge && StatutoryLowAge; }
        }

        public bool OfstedRatingDetails
        {
            get { return OfstedInspectionDate && OfstedRatingId; }
            set { OfstedInspectionDate = OfstedRatingId = value; }
        }

        public bool TypeOfSENProvisionList
        {
            get { return SEN1Id && SEN2Id && SEN3Id && SEN4Id; }
            set { SEN1Id = SEN2Id = SEN3Id = SEN4Id = value; }
        }

        public bool LocationDetails
        {
            get
            {
                return RSCRegionId && GovernmentOfficeRegionId && AdministrativeDistrictId && AdministrativeWardId && ParliamentaryConstituencyId &&
                    UrbanRuralId && GSSLAId && CASWardId && MSOAId && LSOAId && Easting && Northing;
            }
            set { SetLocationFields(value); }
        }
        
        public bool GroupDetails { get; set; }

        public bool CCLAContactDetail { get; set; }
        
        /// <summary>
        /// Alias for Group Details, but displayed in a different place under a different label
        /// </summary>
        public bool GroupCollaborationName { get; set; }

        public void SetLocationFields(bool flag)
        {
            RSCRegionId = GovernmentOfficeRegionId = AdministrativeDistrictId = AdministrativeWardId = ParliamentaryConstituencyId =
                UrbanRuralId = GSSLAId = CASWardId = MSOAId = LSOAId = Easting = Northing = flag;
        }

        public void SetAddressFields(bool flag)
        {
            Address_CityOrTown = flag;
            Address_Country = flag;
            Address_County = flag;
            Address_Line1 = flag;
            Address_Line2 = flag;
            Address_Line3 = flag;
            Address_Locality = flag;
            Address_PostCode = flag;
        }

    }
}
