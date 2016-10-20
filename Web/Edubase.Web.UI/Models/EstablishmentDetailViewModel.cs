using Edubase.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Edubase.Common;

namespace Edubase.Web.UI.Models
{
    public class EstablishmentDetailViewModel
    {
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

        public LinkedEstab[] LinkedEstablishments { get; set; }

        public List<PendingChangeViewModel> PendingChanges { get; set; } = new List<PendingChangeViewModel>();
        public bool ShowPendingMessage { get; set; }

        public bool HasPendingUpdate(string fieldName) => PendingChanges.Any(x => x.DataField.Equals(fieldName));
        public bool UserHasPendingApprovals { get; set; }

        public bool IsUserLoggedOn { get; set; }

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

    }
}