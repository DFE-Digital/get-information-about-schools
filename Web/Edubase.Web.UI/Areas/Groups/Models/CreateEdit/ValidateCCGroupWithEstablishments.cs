using System;
using System.Collections.Generic;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    public class ValidateCCGroupWithEstablishments
    {
        public int GroupTypeId { get; set; }
        public string Name { get; set; }
        public DateTime OpenDate { get; set; }
        public int LocalAuthorityId { get; set; }
        public List<ValidateCCGroupEstablishment> Establishments { get; set; }
    }

    public class ValidateCCGroupEstablishment
    {
        public int Urn { get; set; }
        public DateTime JoinedDate { get; set; }
        public bool CCIsLeadCentre { get; set; }
    }
}