using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.IntegrationEndPoints.CompaniesHouse.Models;
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
        private static readonly Dictionary<int?, string> GroupTypeName = new Dictionary<int?, string>()
        {
            { (int) eLookupGroupType.MultiacademyTrust, "multi-academy trust" },
            { (int) eLookupGroupType.SingleacademyTrust, "single-academy trust" },
            { (int) eLookupGroupType.SecureSingleAcademyTrust, "secure single-academy trust" }
        };

        public CreateAcademyTrustViewModel(CompanyProfile companyProfile)
        {
            Name = companyProfile.Name;
            OpenDate = companyProfile.IncorporationDate;
            CompaniesHouseNumber = companyProfile.Number;
            Address = companyProfile.Address.ToString().Clean();
            CompaniesHouseAddressToken = UriHelper.SerializeToUrlToken(companyProfile.Address);
            TrustExists = false;
        }

        public CreateAcademyTrustViewModel()
        {

        }

        public CreateAcademyTrustViewModel(CompanyProfile companyProfile, IEnumerable<SelectListItem> groupTypes) 
            : this(companyProfile)
        {
            GroupTypes = groupTypes;
        }

        public string Name { get; set; }

        [Required(ErrorMessage = "Please select the trust type"), DisplayName("Trust type")]
        public int? TypeId { get; set; }

        //[Required(ErrorMessage = "Please enter a group ID"), DisplayName("Group ID")]
        //added validation to the controller as this now depends on if the group is 16-19 secure
        public string GroupId { get; set; }

        public int GroupUid { get; set; }

        public DateTime? OpenDate { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public string Address { get; set; }
        public bool TrustExists { get; internal set; }
        public string TrustName { get; internal set; }
        public bool AllowSave { get; set; } = true;
        public IEnumerable<SelectListItem> GroupTypes { get; set; }

        public string CompaniesHouseAddressToken { get; set; }

        public string TypeName => TypeId != null ? GroupTypeName[TypeId] : null;
    }
}
