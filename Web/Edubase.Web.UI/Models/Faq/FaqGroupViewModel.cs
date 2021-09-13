using System.ComponentModel.DataAnnotations;

namespace Edubase.Web.UI.Models.Faq
{
    public class FaqGroupViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Please enter a Group Name")]
        public string GroupName { get; set; }
        public int DisplayOrder { get; set; }

        public bool CanDelete { get; set; }
        public bool IsDeleting { get; set; }
    }
}
