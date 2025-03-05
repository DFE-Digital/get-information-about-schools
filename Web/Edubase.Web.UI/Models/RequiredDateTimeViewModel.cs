using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Edubase.Web.UI.Models
{
    public class RequiredDateTimeViewModel : DateTimeViewModel
    {
        [Required(ErrorMessage = "The day is required.")]
        [DisplayName("Day"), Range(1, 31)]
        public int? Day { get; set; }

        [Required(ErrorMessage = "The month is required.")]
        [DisplayName("Month"), Range(1, 12)]
        public int? Month { get; set; }

        [Required(ErrorMessage = "The year is required.")]
        [DisplayName("Year"), Range(1800, 2100)]
        public int? Year { get; set; }

        [Required(ErrorMessage = "The hour is required.")]
        [DisplayName("Hour"), Range(0, 23), DisplayFormat(DataFormatString = "{0:D2}", ApplyFormatInEditMode = true)]
        public int? Hour { get; set; }

        [Required(ErrorMessage = "The minute is required.")]
        [DisplayName("Minute"), Range(0, 59), DisplayFormat(DataFormatString = "{0:D2}", ApplyFormatInEditMode = true)]
        public int? Minute { get; set; }
    }
}
