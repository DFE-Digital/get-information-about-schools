using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Web.UI.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Castle.Components.DictionaryAdapter;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    using Edubase.Services.Domain;
    using GT = eLookupGroupType;

    public class GroupEditorViewModel : GroupEditorViewModelBase
    {
        public enum eSaveMode
        {
            Details,
            Links,
            DetailsAndLinks
        }

        private static readonly Dictionary<int, string> _groupTypeLabelPrefixers = new Dictionary<int, string>
        {
            [(int) GT.ChildrensCentresCollaboration] = "Children's centres collaboration",
            [(int) GT.ChildrensCentresGroup] = "Children's centres group",
            [(int) GT.Federation] = "Federation",
            [(int) GT.MultiacademyTrust] = "Multi-academy trust",
            [(int) GT.SchoolSponsor] = "Academy sponsor",
            [(int) GT.SingleacademyTrust] = "Single-academy trust",
            [(int) GT.Trust] = "Foundation trust",
            [(int) GT.SecureSingleAcademyTrust] = "Secure single-academy trust",
        };

        public const string ActionSave = "save";
        public const string ActionSaveLinks = "savelinks";
        public const string ActionCcCreate = "continue";
        public const string ActionDetails = "continueToDetails";
        public const string ActionLinkedEstablishmentAdd = "add";
        public const string ActionLinkedEstablishmentCancelAdd = "canceladd";
        public const string ActionLinkedEstablishmentRemove = "remove-";
        public const string ActionLinkedEstablishmentEdit = "edit-";
        public const string ActionLinkedEstablishmentCancelEdit = "cancel";
        public const string ActionLinkedEstablishmentSave = "savejoineddate";
        public const string ActionLinkedEstablishmentSearch = "search";
        public const string ActionLinkedEstablishmentStartSearch = "startsearch";

        public string[] RecognisedWarningCodes { get; set; } = new[]
        {
            ApiWarningCodes.CONFIRMATION_CC_CLOSE,
            ApiWarningCodes.GROUP_WITH_SIMILAR_NAME_FOUND,
            ApiWarningCodes.CONFIRMATION_FEDERATION_NO_LINKS_CLOSE,
            ApiWarningCodes.CONFIRMATION_FEDERATION_BECOMES_CLOSED_LINKS_REMOVED,
            ApiWarningCodes.GROUP_OPEN_DATE_ALIGNMENT,
            ApiWarningCodes.CONFIRMATION_MAT_CLOSE_LINKS,
            ApiWarningCodes.CONFIRMATION_SAT_CLOSE_LINKS
        };

        public string Action { get; set; }
        public int ActionUrn => Action.Split('-').Last().ToInteger() ?? 0;
        public eSaveMode SaveMode { get; set; }
        public string GroupTypeLabelPrefix => GroupTypeId.HasValue ? _groupTypeLabelPrefixers[GroupTypeId.Value] : null;
        public string GroupNameLabel => GroupTypeId.HasValue ? $"{_groupTypeLabelPrefixers[GroupTypeId.Value]} name" : null;
        public bool InEditMode => GroupUId.HasValue;
        public string ManagerEmailAddress { get; set; }
        public int? LocalAuthorityId { get; set; }
        public bool IsLocalAuthorityEditable { get; set; }
        public string LocalAuthorityName { get; set; }
        public string OpenDateLabel => GroupType.OneOfThese(GT.MultiacademyTrust, GT.SingleacademyTrust) ? "Incorporated on (open date)" : "Open date";
        public GT? GroupType => GroupTypeId.HasValue ? (GT) GroupTypeId.Value : null as GT?;

        public string OriginalGroupName { get; set; }
        public string OriginalGroupTypeName { get; set; }


        [Display(Name = "Group ID")]
        public string GroupId { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public string Address { get; set; }
        public DateTimeViewModel OpenDate { get; set; } = new DateTimeViewModel();
        public string AddressJsonToken { get; set; }
        public bool CloseAndMarkAsCreatedInError { get; set; }
        public bool CanUserCloseAndMarkAsCreatedInError { get; set; }
        public string CloseAndMarkAsCreatedInErrorLabel => CanUserCloseAndMarkAsCreatedInError ? $"Close this {GroupTypeLabelPrefix.ToLower()} and mark as created in error" : null;
        public bool CanUserEditStatus { get; set; }
        public bool CanUserEditClosedDate { get; set; }
        public bool CanUserEditUkprn { get; set; }

        public IEnumerable<SelectListItem> Statuses { get; set; }

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

        public const string UIWarningCodeMatClosureAreYouSure = "mat_closure_are_you_sure";
        public const string UIWarningCodeSatClosureAreYouSure = "sat_closure_are_you_sure";

        public List<ApiWarning> WarningsToProcess { get; private set; } = new List<ApiWarning>();

        public bool ProcessedWarnings { get; set; }

        public void SetWarnings(ValidationEnvelopeDto envelope)
        {
            if (!ProcessedWarnings && !envelope.Errors.Any())
            {
                WarningsToProcess = envelope.Warnings ?? new List<ApiWarning>();
                WarningsToProcess = WarningsToProcess.Where(x => RecognisedWarningCodes.Contains(x.Code)).ToList();
                ProcessedWarnings = true;

                if (OriginalStatusId != (int) eLookupGroupStatus.Closed && StatusId == (int) eLookupGroupStatus.Closed && GroupTypeId == (int) GT.MultiacademyTrust)
                {
                    WarningsToProcess.Add(new ApiWarning { Code = UIWarningCodeMatClosureAreYouSure, Message = "Are you sure you want to close this multi-academy trust?" });
                }

                if (OriginalStatusId != (int) eLookupGroupStatus.Closed && StatusId == (int) eLookupGroupStatus.Closed && GroupTypeId == (int) GT.SingleacademyTrust)
                {
                    WarningsToProcess.Add(new ApiWarning { Code = UIWarningCodeSatClosureAreYouSure, Message = "Are you sure you want to close this single-academy trust?" });
                }
            }
        }

        public List<ChangeDescriptorDto> ChangesSummary { get; set; }
        public bool ChangesAcknowledged { get; set; }

        public void ClearWarnings()
        {
            WarningsToProcess = new EditableList<ApiWarning>();
        }

        public GroupEditorViewModel()
        {

        }

        public GroupEditorViewModel(eSaveMode saveMode)
        {
            SaveMode = SaveMode;
        }

    }
}
