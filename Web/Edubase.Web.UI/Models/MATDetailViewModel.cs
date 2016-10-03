using Edubase.Data.Entity;
using Edubase.Data.Stubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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