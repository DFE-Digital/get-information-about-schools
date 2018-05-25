using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Edubase.Web.UI.Models
{
    public class GlossaryItemViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required, MaxLength(1000, ErrorMessage = "The Content field cannot have more than 1000 characters"), AllowHtml]
        public string Content { get; set; }

        public bool IsDeleting { get; set; }

        public GlossaryItemViewModel()
        {

        }
    }
}