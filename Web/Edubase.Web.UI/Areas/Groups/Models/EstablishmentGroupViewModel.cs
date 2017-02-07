using Edubase.Common;
using Edubase.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Areas.Groups.Models
{
    public class EstablishmentGroupViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int Urn { get; set; }
        public string Address { get; set; }
        public string TypeName { get; set; }
        public string HeadFirstName { get; set; }
        public string HeadLastName { get; set; }
        public DateTime? JoinedDate { get; set; }
        public DateTimeViewModel JoinedDateEditable { get; set; } = new DateTimeViewModel();
        public string HeadTitleName { get; set; }
        public bool EditMode { get; set; }

        /// <summary>
        /// Where this is a Children's Centre establishment; this flag denotes whether this establishment is the 'lead centre'
        /// </summary>
        public bool CCIsLeadCentre { get; set; }

        public string HeadFullName => StringUtil.ConcatNonEmpties(" ", HeadTitleName, HeadFirstName, HeadLastName);

        public EstablishmentGroupViewModel SetEditMode(bool flag = true)
        {
            EditMode = flag;
            return this;
        }
    }
}