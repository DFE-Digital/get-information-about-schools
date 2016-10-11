using Edubase.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Models
{
    public class EstablishmentDetailViewModel
    {
        public Establishment Establishment { get; set; }
        public List<PendingChangeViewModel> PendingChanges { get; set; } = new List<PendingChangeViewModel>();
        public bool ShowPendingMessage { get; set; }

        public bool HasPendingUpdate(string fieldName) => PendingChanges.Any(x => x.DataField.Equals(fieldName));
        public bool UserHasPendingApprovals { get; set; }

        public bool IsUserLoggedOn { get; set; }
    }
}