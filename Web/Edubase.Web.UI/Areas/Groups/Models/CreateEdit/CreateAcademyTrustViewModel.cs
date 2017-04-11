using Edubase.Common;
using Edubase.Services.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    public class CreateAcademyTrustViewModel
    {
        public CreateAcademyTrustViewModel(CompanyProfileDto companyProfile)
        {
            Name = companyProfile.Name;
            OpenDate = companyProfile.IncorporationDate;
            CompaniesHouseNumber = companyProfile.Number;
            Address = companyProfile.Address.ToString();
            CompaniesHouseAddressToken = UriHelper.SerializeToUrlToken(companyProfile.Address);
            TrustExists = false;
        }

        public CreateAcademyTrustViewModel()
        {

        }

        public CreateAcademyTrustViewModel(CompanyProfileDto companyProfile, IEnumerable<SelectListItem> groupTypes) 
            : this(companyProfile)
        {
            GroupTypes = groupTypes;
        }

        public string Name { get; set; }

        [Required, DisplayName("Trust type")]
        public int? TypeId { get; set; }

        [Required, DisplayName("Group ID")]
        public string GroupId { get; set; }

        public DateTime? OpenDate { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public string Address { get; set; }
        public bool TrustExists { get; internal set; }
        public IEnumerable<SelectListItem> GroupTypes { get; set; }

        public string CompaniesHouseAddressToken { get; set; }


    }
}