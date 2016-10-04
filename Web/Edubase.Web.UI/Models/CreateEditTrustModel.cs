using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class CreateEditTrustModel
    {
        public int? GroupUID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int? TypeId { get; set; }
        public DateTimeViewModel OpenDate { get; set; }
    }
}