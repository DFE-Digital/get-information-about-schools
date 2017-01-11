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

namespace Edubase.Web.UI.Models
{
    public class EstablishmentDetailViewModel
    {
        private static Dictionary<int, string> _groupType2FieldLabelMappings = new Dictionary<int, string>
        {
            [(int)eLookupGroupType.SingleacademyTrust] = "Single Academy trust",
            [(int)eLookupGroupType.MultiacademyTrust] = "Academy trust",
            [(int)eLookupGroupType.SchoolSponsor] = "Academy sponsor",
            //[eLookupGroupType.cosponsor] = "Academy co-sponsor" //todo: need to add that one to the lookup on DB seeding
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

        public GroupModel Group { get; set; }

        public IEnumerable<EstablishmentChangeDto> ChangeHistory { get; set; }

        public IEnumerable<LinkedEstabViewModel> LinkedEstablishments { get; set; }

        public List<PendingChangeViewModel> PendingChanges { get; set; } = new List<PendingChangeViewModel>();
        public bool ShowPendingMessage { get; set; }

        public bool HasPendingUpdate(string fieldName) => PendingChanges.Any(x => x.DataField.Equals(fieldName));
        public bool UserHasPendingApprovals { get; set; }

        public bool IsUserLoggedOn { get; set; }

        public bool UserCanEdit { get; set; }

        public IEnumerable<Governor> Governors { get; set; }
        public IEnumerable<Governor> HistoricalGovernors { get; set; }
        
        public string GroupFieldLabel => Group != null ? _groupType2FieldLabelMappings.Get(Group.GroupTypeId.GetValueOrDefault()) : string.Empty;

        public bool IsClosed => Establishment.StatusId == (int)eLookupEstablishmentStatus.Closed;

        public EstablishmentDetailViewModel()
        {

        }

        public string OfstedRatingReportUrl => (Establishment.OfstedRating.HasValue ? $"http://www.ofsted.gov.uk/oxedu_providers/full/(urn)/{Establishment.Urn}" : null as string);
        
    }
}