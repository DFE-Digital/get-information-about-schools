using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Data.Entity
{
    public class EstablishmentPermission
    {
        [Key, Column(Order = 1)]
        public string PropertyName { get; set; }
        [Key, Column(Order = 2)]
        public string RoleName { get; set; }
        public bool AllowUpdate { get; set; }
        public bool AllowApproval { get; set; }

    }
}
