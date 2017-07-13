using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Web.UI.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    using System;
    using GT = eLookupGroupType;

    public class GroupEditorViewModel : GroupEditorViewModelBase
    {
        public enum eSaveMode
        {
            Details,
            Links,
            DetailsAndLinks
        }
        
        private static readonly Dictionary<eGroupTypeMode, string> _fieldNamePrefixers = new Dictionary<eGroupTypeMode, string>
        {
            [eGroupTypeMode.ChildrensCentre] = "Group",
            [eGroupTypeMode.Trust] = "Trust",
            [eGroupTypeMode.Federation] = "Federation",
            [eGroupTypeMode.AcademyTrust] = "Trust",
            [eGroupTypeMode.Sponsor] = "Academy sponsor"
        };

        public const string ActionSave = "save";
        public const string ActionSaveLinks = "savelinks";
        public const string ActionLinkedEstablishmentAdd = "add";
        public const string ActionLinkedEstablishmentRemove = "remove-";
        public const string ActionLinkedEstablishmentEdit = "edit-";
        public const string ActionLinkedEstablishmentCancelEdit = "cancel";
        public const string ActionLinkedEstablishmentSave = "savejoineddate";
        public const string ActionLinkedEstablishmentSearch = "search";

        public string Action { get; set; }
        public int ActionUrn => int.Parse(Action.Split('-')[1]);
        public eSaveMode SaveMode { get; set; }
        public string FieldNamePrefix => _fieldNamePrefixers.Get(GroupTypeMode);
        public bool InEditMode => GroupUId.HasValue;
        public string ManagerEmailAddress { get; set; }
        public int? LocalAuthorityId { get; set; }
        public bool IsLocalAuthorityEditable { get; set; }
        public string LocalAuthorityName { get; set; }
        public string GroupTypeName { get; set; }
        public string OpenDateLabel => GroupType.OneOfThese(GT.MultiacademyTrust, GT.SingleacademyTrust) ? "Incorporated on" : "Open date";
        public GT? GroupType => GroupTypeId.HasValue ? (GT)GroupTypeId.Value : null as GT?;
        public string GroupId { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public string Address { get; set; }
        public DateTimeViewModel OpenDate { get; set; } = new DateTimeViewModel();
        public string AddressJsonToken { get; set; }

        [Display(Name = "Closed date")]
        public DateTimeViewModel ClosedDate { get; set; } = new DateTimeViewModel();

        public int? CCLeadCentreUrn { get; set; }

        public void SetCCLeadCentreUrn()
        {
            if (GroupType == GT.ChildrensCentresCollaboration || GroupType == GT.ChildrensCentresGroup)
            {
                if (CCLeadCentreUrn.HasValue && LinkedEstablishments.Establishments.Count > 0)
                {
                    LinkedEstablishments.Establishments.ForEach(x => x.CCIsLeadCentre = false);
                    LinkedEstablishments.Establishments.Single(x => x.Urn == CCLeadCentreUrn).CCIsLeadCentre = true;
                }
            }
        }

        public void DeriveCCLeadCentreUrn()
        {
            CCLeadCentreUrn = LinkedEstablishments.Establishments.Where(x => x.CCIsLeadCentre).Select(x => new int?(x.Urn)).SingleOrDefault();
        }

        /// <summary>
        /// Whether the UI mode requires the Establishment links are saved.
        /// </summary>
        public bool SaveEstabLinks => SaveMode == eSaveMode.DetailsAndLinks || SaveMode == eSaveMode.Links;
        

        /// <summary>
        /// Whether the UI mode requires the Group detail is saved.
        /// </summary>
        public bool SaveGroupDetail => SaveMode == eSaveMode.DetailsAndLinks || SaveMode == eSaveMode.Details;
        
        public GroupLinkedEstablishmentsViewModel LinkedEstablishments { get; set; } = new GroupLinkedEstablishmentsViewModel();

        /// <summary>
        /// Children's centre group types
        /// </summary>
        public IEnumerable<SelectListItem> CCGroupTypes { get; set; }
        

        public IEnumerable<SelectListItem> LocalAuthorities { get; set; }


        public GroupEditorViewModel()
        {

        }
        
        public GroupEditorViewModel(eSaveMode saveMode)
        {
            SaveMode = SaveMode;
        }

    }
}