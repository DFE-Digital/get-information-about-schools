using Edubase.Data.Entity;
using System.Collections.Generic;

namespace Edubase.Web.UI.Models
{
    public class MATDetailViewModel
    {

        public List<Establishment2Company> Establishments { get; set; }
        public Trust MAT { get; set; }
        public bool IsUserLoggedOn { get; set; }

        public MATDetailViewModel(List<Establishment2Company> estabs, Trust mat, bool isUserLoggedOn)
        {
            MAT = mat;
            Establishments = estabs;
            mat.EstablishmentCount = estabs.Count;
            IsUserLoggedOn = isUserLoggedOn;
        }
    }
}