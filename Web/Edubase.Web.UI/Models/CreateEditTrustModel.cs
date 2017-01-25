using Edubase.Data.Entity;
using Edubase.Web.UI.Models.Validators;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Edubase.Web.UI.Models
{
    [Validator(typeof(CreateEditGroupModelValidator))]
    public class CreateEditGroupModel
    {
        public int? GroupUID { get; set; }
        public string GroupId { get; set; }
        public string Name { get; set; }
        public int? TypeId { get; set; }
        public DateTime? OpenDate { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public string Address { get; set; }
        public string SearchURN { get; set; }
        public string EstablishmentName { get; set; }
        public int? EstablishmentUrn { get; set; }
        public bool EstablishmentNotFound { get; set; }
        public int? EstabUrnToRemove { get; set; }
        public DateTimeViewModel JoinedDate { get; set; } = new DateTimeViewModel();
        public string TypeName { get; set; }

        public string Action { get; set; }

        public List<GroupEstabViewModel> Establishments { get; set; } = new List<GroupEstabViewModel>();
    }
}