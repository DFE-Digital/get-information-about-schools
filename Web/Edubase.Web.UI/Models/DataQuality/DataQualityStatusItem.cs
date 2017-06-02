using Edubase.Data.Entity;

namespace Edubase.Web.UI.Models.DataQuality
{
    public class DataQualityStatusItem
    {
        public DataQualityStatus.DataQualityEstablishmentType EstablishmentType { get; set; }
        public DateTimeViewModel LastUpdated { get; set; }
    }
}