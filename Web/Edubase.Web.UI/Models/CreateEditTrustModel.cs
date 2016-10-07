using Edubase.Web.UI.Models.Validators;
using FluentValidation.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Edubase.Web.UI.Models
{
    [Validator(typeof(CreateEditTrustModelValidator))]
    public class CreateEditTrustModel
    {
        public int? GroupUID { get; set; }
        public string Name { get; set; }
        public int? TypeId { get; set; }
        public DateTimeViewModel OpenDate { get; set; }
    }
}