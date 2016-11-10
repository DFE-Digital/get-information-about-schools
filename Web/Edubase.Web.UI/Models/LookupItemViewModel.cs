using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;
using Edubase.Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class LookupItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public LookupItemViewModel()
        {

        }

        public LookupItemViewModel(LookupBase item)
        {
            Id = item.Id;
            Name = item.Name;
        }
        public LookupItemViewModel(LookupDto item)
        {
            Id = item.Id;
            Name = item.Name;
        }

        public LookupItemViewModel(LocalAuthority item)
        {
            Id = item.Id;
            Name = item.Name;
        }

        public LookupItemViewModel(LookupDiocese item)
        {
            Id = item.Id;
            Name = item.Name;
        }

        public override string ToString() => Name;
    }
}