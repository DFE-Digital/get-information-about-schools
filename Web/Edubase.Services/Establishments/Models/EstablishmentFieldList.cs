namespace Edubase.Services.Establishments.Models
{
    /// <summary>
    /// Represents the list of fields within an Establishment.
    /// </summary>
    public class EstablishmentFieldList : EstablishmentFieldListBase
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

        
        public bool GroupDetails { get; set; }

        public bool CCLAContactDetail { get; set; }
        
        /// <summary>
        /// Alias for Group Details, but displayed in a different place under a different label
        /// </summary>
        public bool GroupCollaborationName { get; set; }
    }
}
