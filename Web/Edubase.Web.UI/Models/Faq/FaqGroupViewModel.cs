using System.ComponentModel.DataAnnotations;

namespace Edubase.Web.UI.Models.Faq
{
    public class FaqGroupViewModel
    {
        public string Id { get; set; }

        [Required]
        public string GroupName { get; set; }
        public int DisplayOrder { get; set; }

        public bool CanDelete { get; set; }
        public bool IsDeleting { get; set; }
    }
}
