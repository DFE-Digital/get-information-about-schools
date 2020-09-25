using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Edubase.Web.UI.Models
{
    public class FaqItemViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required, MaxLength(4000, ErrorMessage = "The Content field cannot have more than 4000 characters"), AllowHtml]
        public string Content { get; set; }

        public bool IsDeleting { get; set; }

        [Required, Display(Name = "Display order")]
        public int DisplayOrder { get; set; }

        [Required, Display(Name = "Title font size")]
        public string TitleFontSize { get; set; }

        public FaqItemViewModel()
        {
        }
    }
}
