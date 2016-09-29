using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Data.Entity.ComplexTypes
{
    [ComplexType]
    public class ContactDetail
    {
        public string TelephoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string WebsiteAddress { get; set; }
    }
}
