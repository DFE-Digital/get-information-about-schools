using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class GlossaryItemViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required, MaxLength(1000, ErrorMessage = "the Content field cannot have more than 1000 characters")]
        public string Content { get; set; }

        public bool IsDeleting { get; set; }

        public GlossaryItemViewModel()
        {

        }
    }
}