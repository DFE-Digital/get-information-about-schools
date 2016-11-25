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

        public Governor[] Govs { get; set; }

        public Governor[] AccountingOfficers => Govs.Where(x => x.RoleId == (int)GovRole.AccountingOfficer).ToArray();
        public Governor[] ChairsOfGovernors => Govs.Where(x => x.RoleId == (int)GovRole.ChairOfGovernors).ToArray();
        public Governor[] ChairsOfLocalGoverningBody => Govs.Where(x => x.RoleId == (int)GovRole.ChairOfLocalGoverningBody).ToArray();
        public Governor[] ChairsOfTrustees => Govs.Where(x => x.RoleId == (int)GovRole.ChairOfTrustees).ToArray();
        public Governor[] ChiefFinancialOfficers => Govs.Where(x => x.RoleId == (int)GovRole.ChiefFinancialOfficer).ToArray();
        public Governor[] Governors => Govs.Where(x => x.RoleId == (int)GovRole.Governor).ToArray();
        public Governor[] LocalGovernors => Govs.Where(x => x.RoleId == (int)GovRole.LocalGovernor).ToArray();
        public Governor[] Members => Govs.Where(x => x.RoleId == (int)GovRole.Member).ToArray();
        public Governor[] Trustees => Govs.Where(x => x.RoleId == (int)GovRole.Trustee).ToArray();

        public Governor[] Historic(Governor[] govs) => 
            govs.Where(x => x.AppointmentEndDate != null 
            && x.AppointmentEndDate.Value > DateTime.UtcNow.Date.AddYears(-1) 
            && x.AppointmentEndDate < DateTime.UtcNow.Date).ToArray();

        public Governor[] NonHistoric(Governor[] govs) => govs.Where(x => 
            !x.AppointmentEndDate.HasValue 
            || x.AppointmentEndDate.IsInFuture()).ToArray();




        public string GroupFieldLabel => Group != null ? _groupType2FieldLabelMappings.Get(Group.GroupTypeId.GetValueOrDefault()) : string.Empty;

        public bool IsClosed => Establishment.StatusId == (int)eLookupEstablishmentStatus.Closed;

        public EstablishmentDetailViewModel(IPrincipal user)
        {
            _displayProfile = new Lazy<EstablishmentDisplayProfile>(() => new DisplayProfileFactory().Get(user, Establishment, Group));
        }

        public string OfstedRatingReportUrl => (Establishment.OfstedRating.HasValue ? $"http://www.ofsted.gov.uk/oxedu_providers/full/(urn)/{Establishment.Urn}" : null as string);

    }
}