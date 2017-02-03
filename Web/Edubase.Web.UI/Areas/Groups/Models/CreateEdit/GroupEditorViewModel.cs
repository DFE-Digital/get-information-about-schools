using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    using Exceptions;
    using GT = eLookupGroupType;

    public class GroupEditorViewModel
    {
        public enum eGroupTypeMode
        {
            Federation,
            ChildrensCentre,
            Trust,
            Academy
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

        private static readonly Dictionary<eGroupTypeMode, string> _pluralEstablishmentNames = new Dictionary<eGroupTypeMode, string>
        {
            [eGroupTypeMode.ChildrensCentre] = "children's centres",
            [eGroupTypeMode.Trust] = "schools",
            [eGroupTypeMode.Federation] = "schools",
            [eGroupTypeMode.Academy] = "academies"
        };

        public const string ActionSave = "save";
        public const string ActionLinkedEstablishmentAdd = "add";
        public const string ActionLinkedEstablishmentRemove = "remove-";
        public const string ActionLinkedEstablishmentEdit = "edit-";
        public const string ActionLinkedEstablishmentCancelEdit = "cancel";
        public const string ActionLinkedEstablishmentSave = "savejoineddate";
        public const string ActionLinkedEstablishmentSearch = "search";

        public string Action { get; set; }

        public int ActionUrn => int.Parse(Action.Split('-')[1]);

        public eGroupTypeMode GroupTypeMode { get; set; }

        public string EntityName => _entityNames.Get(GroupTypeMode);

        public string FieldNamePrefix => _fieldNamePrefixers.Get(GroupTypeMode);

        public string ListOfEstablishmentsPluralName =>  GroupType.OneOfThese(GT.ChildrensCentresCollaboration, GT.ChildrensCentresGroup) ? "children's centres" : "establishments";

        public string PageTitle => string.Concat(GroupUID.HasValue ? "Edit " : "Create new ", EntityName);

        public bool InEditMode => GroupUID.HasValue;

        public bool AllowUserToAddLinkedEstablishments => GroupUID.HasValue || GroupTypeMode == eGroupTypeMode.ChildrensCentre;

        public int? GroupUID { get; set; }
        public int? GroupTypeId { get; set; }
        public int? GroupStatusId { get; set; }
        public string GroupManagerEmailAddress { get; set; }
        public int? LocalAuthorityId { get; set; }
        public bool IsLocalAuthorityEditable { get; set; }
        public string LocalAuthorityName { get; set; }

        public GT? GroupType => GroupTypeId.HasValue ? (GT) GroupTypeId.Value : null as GT?;

        public string Name { get; set; }
        public string GroupId { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public string Address { get; set; }
        public DateTimeViewModel OpenDate { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel ClosedDate { get; set; } = new DateTimeViewModel();

        public int? CCLeadCentreUrn { get; set; }

        public void SetCCLeadCentreUrn()
        {
            LinkedEstablishments.Establishments.ForEach(x => x.CCIsLeadCentre = false);
            LinkedEstablishments.Establishments.Single(x => x.Urn == CCLeadCentreUrn).CCIsLeadCentre = true;
        }

        public void GetCCLeadCentreUrn()
        {
            LinkedEstablishments.Establishments.ForEach(x => x.CCIsLeadCentre = false);
            CCLeadCentreUrn = LinkedEstablishments.Establishments.Where(x => x.CCIsLeadCentre).Select(x => new int?(x.Urn)).SingleOrDefault();
        }


        public GroupLinkedEstablishmentsViewModel LinkedEstablishments { get; set; } = new GroupLinkedEstablishmentsViewModel();

        /// <summary>
        /// Children's centre group types
        /// </summary>
        public IEnumerable<SelectListItem> CCGroupTypes { get; set; }

        public IEnumerable<SelectListItem> Statuses { get; set; }

        public IEnumerable<SelectListItem> LocalAuthorities { get; set; }


        public GroupEditorViewModel()
        {

        }

        public GroupEditorViewModel(eGroupTypeMode mode)
        {
            GroupTypeMode = mode;
        }




    }
}