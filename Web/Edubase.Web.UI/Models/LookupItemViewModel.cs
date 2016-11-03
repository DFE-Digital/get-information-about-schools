using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class LookupItemViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public LookupItemViewModel()
        {

        }

        public LookupItemViewModel(LookupBase item)
        {
            Id = item.Id.ToString();
            Name = item.Name;
        }

        public LookupItemViewModel(LocalAuthority item)
        {
            Id = item.Id.ToString();
            Name = item.Name;
        }

        public LookupItemViewModel(Diocese item)
        {
            Id = item.Id;
            Name = item.Name;
        }

        public override string ToString() => Name;
    }
}