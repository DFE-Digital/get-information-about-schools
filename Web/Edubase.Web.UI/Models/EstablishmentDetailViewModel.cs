using Edubase.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using System.Security.Principal;
using Edubase.Services.Groups.Models;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Web.UI.Models.Establishments;
using System.Web;
using System.Linq.Expressions;
using System.Web.Mvc;
using Edubase.Services.Governors.Models;

namespace Edubase.Web.UI.Models
{
    public class EstablishmentDetailViewModel
    {
        private static Dictionary<int, string> _groupType2FieldLabelMappings = new Dictionary<int, string>
        {
            [(int)eLookupGroupType.SingleacademyTrust] = "Single academy trust",
            [(int)eLookupGroupType.MultiacademyTrust] = "Academy trust",
            [(int)eLookupGroupType.SchoolSponsor] = "Academy sponsor",
            [(int)eLookupGroupType.Trust] = "Trust",
            [(int)eLookupGroupType.Federation] = "Federation"
        };

        public EstablishmentDisplayPolicy DisplayPolicy { get; set; }

        public TabDisplayPolicy TabDisplayPolicy { get; set; }

        public enum GovRole
        {
            AccountingOfficer = 1,
            ChairOfGovernors,
            ChairOfLocalGoverningBody,
            ChairOfTrustees,
            ChiefFinancialOfficer,
            Governor,
            LocalGovernor,
            Member,
            Trustee
        }

        public EstablishmentModel Establishment { get; set; }

        public IEnumerable<GroupModel> Groups { get; set; }

        public IEnumerable<EstablishmentChangeDto> ChangeHistory { get; set; }

        public IEnumerable<LinkedEstabViewModel> LinkedEstablishments { get; set; }
        
        
        public bool IsUserLoggedOn { get; set; }

        public bool UserCanEdit { get; set; }

        public GovernorsGridViewModel GovernorsDetails { get; set; }
        
        public bool IsClosed => Establishment.StatusId == (int)eLookupEstablishmentStatus.Closed;

        public EstablishmentDetailViewModel()
        {

        }

        public string OfstedRatingReportUrl => (Establishment.OfstedRating.HasValue 
            ? new OfstedRatingUrl(Establishment.Urn).ToString() : null as string);

        public string GetGroupFieldLabel(GroupModel model) => _groupType2FieldLabelMappings[model.GroupTypeId.Value];
        
    }
}