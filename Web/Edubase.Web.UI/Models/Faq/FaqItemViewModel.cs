using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Edubase.Data.Entity;

namespace Edubase.Web.UI.Models
{
    public class FaqItemViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required, MaxLength(4000, ErrorMessage = "The Content field cannot have more than 4000 characters")]
        public string Content { get; set; }

        public bool IsDeleting { get; set; }

        [Required(ErrorMessage = "Please select a Group")]
        public string GroupId { get; set; }

        public IEnumerable<FaqGroup> Groups { get; set; }

        public FaqItemViewModel()
        {
            
        }
        public FaqItemViewModel(IEnumerable<FaqGroup> groups)
        {
            Groups = groups;
        }
    }
}
