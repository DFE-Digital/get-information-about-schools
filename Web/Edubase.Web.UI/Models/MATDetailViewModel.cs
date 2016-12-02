using Edubase.Data.Entity;
using Edubase.Services.Domain;
using System.Collections.Generic;

namespace Edubase.Web.UI.Models
{
    public class MATDetailViewModel
    {

        public List<EstablishmentGroup> Establishments { get; set; }
        public GroupCollection Group { get; set; }
        public bool IsUserLoggedOn { get; set; }
        public LookupDto LocalAuthority { get; set; }

        public IEnumerable<Governor> Governors { get; set; }
        public IEnumerable<Governor> HistoricalGovernors { get; set; }

        public MATDetailViewModel(List<EstablishmentGroup> estabs, GroupCollection mat, bool isUserLoggedOn, LookupDto localAuthority)
        {
            Group = mat;
            Establishments = estabs;
            mat.EstablishmentCount = estabs.Count;
            IsUserLoggedOn = isUserLoggedOn;
            LocalAuthority = localAuthority;
        }
    }
}