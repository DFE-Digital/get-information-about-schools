using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Groups.Models
{
    public class LinkedGroupModel
    {
        public int UId { get; set; }
        public string LinkType { get; set; }
        public DateTime? LinkDate { get; set; }
        public string GroupName { get; set; }
    }
}
