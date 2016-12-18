using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Data.Entity.ComplexTypes
{
    [ComplexType, Serializable]
    public class ContactDetail
    {
        public string TelephoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string WebsiteAddress { get; set; }
        public string FaxNumber { get; set; }
    }
}
