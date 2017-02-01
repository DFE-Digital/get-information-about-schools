using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Areas.Groups.Models
{
    public class GroupEditorViewModel
    {
        public enum eGroupTypeMode
        {
            Federation,
            ChildrensCentre,
            Trust
        }

        private static readonly Dictionary<eGroupTypeMode, string> _entityNames = new Dictionary<eGroupTypeMode, string>
        {
            [eGroupTypeMode.ChildrensCentre] = "children's centre group or collaboration",
            [eGroupTypeMode.Trust] = "school trust",
            [eGroupTypeMode.Federation] = "school federation"
        };

        private static readonly Dictionary<eGroupTypeMode, string> _fieldNamePrefixers = new Dictionary<eGroupTypeMode, string>
        {
            [eGroupTypeMode.ChildrensCentre] = "Group",
            [eGroupTypeMode.Trust] = "Trust",
            [eGroupTypeMode.Federation] = "Federation"
        };



        public eGroupTypeMode GroupTypeMode { get; internal set; }

        public string EntityName => _entityNames.Get(GroupTypeMode);

        public string FieldNamePrefix => _fieldNamePrefixers.Get(GroupTypeMode);

        public string PageTitle => string.Concat(GroupUID.HasValue ? "Edit " : "Create new ", EntityName);

        public bool InEditMode => GroupUID.HasValue;

        public int? GroupUID { get; internal set; }
        public int? GroupTypeId { get; set; }
        public int? GroupStatusId { get; set; }
        public string GroupManagerEmailAddress { get; set; }
        public int? LocalAuthorityId { get; set; }
        public bool IsLocalAuthorityEditable { get; set; }
        public string LocalAuthorityName { get; set; }

        public eLookupGroupType? GroupType => GroupTypeId.HasValue ? (eLookupGroupType) GroupTypeId.Value : null as eLookupGroupType?;

        public string Name { get; set; }
        public DateTimeViewModel OpenDate { get; set; } = new DateTimeViewModel();

        /// <summary>
        /// Children's centre group types
        /// </summary>
        public IEnumerable<SelectListItem> CCGroupTypes { get; set; }

        public IEnumerable<SelectListItem> Statuses { get; set; }

        public IEnumerable<SelectListItem> LocalAuthorities { get; set; }



    }
}