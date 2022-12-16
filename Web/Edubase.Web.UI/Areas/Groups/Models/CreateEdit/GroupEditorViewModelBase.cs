using Edubase.Common;
using Edubase.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    using Services.Groups.Models;
    using GT = eLookupGroupType;

    public class GroupEditorViewModelBase : IGroupPageViewModel
    {
        public enum eGroupTypeMode
        {
            Federation, // Federation
            ChildrensCentre, // Group or Collaboration
            Trust, // School trust
            AcademyTrust, // MATs and SATs,
            Sponsor // Academy sponsor
        }

        private static readonly Dictionary<eGroupTypeMode, string> _entityNames = new Dictionary<eGroupTypeMode, string>
        {
            [eGroupTypeMode.ChildrensCentre] = "children's centre groups or children's centre collaborations",
            [eGroupTypeMode.Trust] = "foundation trust",
            [eGroupTypeMode.Federation] = "federation",
            [eGroupTypeMode.AcademyTrust] = "academy trusts",
            [eGroupTypeMode.Sponsor] = "academy sponsor"
        };

        public enum eChildrensCentreActions
        {
            Step1,
            Step2,
            Step3,
            Step4
        }

        public eGroupTypeMode GroupTypeMode
        {
            get
            {
                if (GroupTypeId.OneOfThese(GT.ChildrensCentresCollaboration, GT.ChildrensCentresGroup))
                {
                    return eGroupTypeMode.ChildrensCentre;
                }
                else if (GroupTypeId.OneOfThese(GT.Federation))
                {
                    return eGroupTypeMode.Federation;
                }
                else if (GroupTypeId.OneOfThese(GT.Trust))
                {
                    return eGroupTypeMode.Trust;
                }
                else if (GroupTypeId.OneOfThese(GT.SchoolSponsor))
                {
                    return eGroupTypeMode.Sponsor;
                }
                else if (GroupTypeId.OneOfThese(GT.MultiacademyTrust, GT.SingleacademyTrust))
                {
                    return eGroupTypeMode.AcademyTrust;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(GroupTypeId),
                        $@"GroupTypeId '{GroupTypeId}' is not supported");
                }
            }
        }

        public eChildrensCentreActions ActionName { get; set; }

        public string ListOfEstablishmentsPluralName { get; set; }
        public string PageTitle => string.Concat(GroupUId.HasValue ? "Edit " : "Create new ", EntityName);
        public string EntityName => _entityNames.Get(GroupTypeMode);

        public string Layout { get; set; }

        public int? GroupUId { get; set; }
        public string GroupName { get; set; }
        public int? GroupTypeId { get; set; }
        public string GroupTypeName { get; set; }
        public string SelectedTabName { get; set; }
        public int? StatusId { get; set; }
        public int? OriginalStatusId { get; set; }
        public int? UKPRN { get; set; }

        public string CCTypeName => GroupTypeMode == eGroupTypeMode.ChildrensCentre ? GroupTypeName?.Split(' ').Last() ?? string.Empty : string.Empty;
    }
}
