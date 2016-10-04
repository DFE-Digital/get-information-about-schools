using Edubase.Data.Entity;
using System.Collections.Generic;

namespace Edubase.Web.UI.Models
{
    public class MATDetailViewModel
    {

        public List<Establishment2Company> Establishments { get; set; }
        public Company MAT { get; set; }

        public MATDetailViewModel(List<Establishment2Company> estabs, Company mat)
        {
            MAT = mat;
            Establishments = estabs;
            mat.EstablishmentCount = estabs.Count;
        }
    }
}