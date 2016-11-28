using Edubase.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Web.UI.Models.DisplayProfiles;
using System.Security.Principal;

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

        private Lazy<EstablishmentDisplayProfile> _displayProfile = null;

        public EstablishmentDisplayProfile DisplayProfile => _displayProfile.Value;

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

        public Establishment Establishment { get; set; }

        public GroupCollection Group { get; set; }

        public EstablishmentChangeDto[] ChangeHistory { get; set; }

        public LinkedEstabViewModel[] LinkedEstablishments { get; set; }

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

        public EstablishmentDetailViewModel(IPrincipal user)
        {
            _displayProfile = new Lazy<EstablishmentDisplayProfile>(() => new DisplayProfileFactory().Get(user, Establishment, Group));
        }

        public string OfstedRatingReportUrl => (Establishment.OfstedRating.HasValue ? $"http://www.ofsted.gov.uk/oxedu_providers/full/(urn)/{Establishment.Urn}" : null as string);

    }
}