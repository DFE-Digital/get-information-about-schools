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
            AcademyTrust // MATs and SATs
        }

        private static readonly Dictionary<eGroupTypeMode, string> _pluralEstablishmentNames = new Dictionary<eGroupTypeMode, string>
        {
            [eGroupTypeMode.ChildrensCentre] = "children's centres",
            [eGroupTypeMode.Trust] = "schools",
            [eGroupTypeMode.Federation] = "schools",
            [eGroupTypeMode.AcademyTrust] = "academies"
        };

        private static readonly Dictionary<eGroupTypeMode, string> _entityNames = new Dictionary<eGroupTypeMode, string>
        {
            [eGroupTypeMode.ChildrensCentre] = "children's centre group or collaboration",
            [eGroupTypeMode.Trust] = "school trust",
            [eGroupTypeMode.Federation] = "school federation",
            [eGroupTypeMode.AcademyTrust] = "Academy Trust"
        };

        public eGroupTypeMode GroupTypeMode
        {
            get
            {
                if (GroupTypeId.OneOfThese(GT.ChildrensCentresCollaboration, GT.ChildrensCentresGroup)) return eGroupTypeMode.ChildrensCentre;
                else if (GroupTypeId.OneOfThese(GT.Federation)) return eGroupTypeMode.Federation;
                else if (GroupTypeId.OneOfThese(GT.Trust)) return eGroupTypeMode.Trust;
                else if (GroupTypeId.OneOfThese(GT.MultiacademyTrust, GT.SingleacademyTrust)) return eGroupTypeMode.AcademyTrust;
                else throw new NotImplementedException();
            }
        }

        public string ListOfEstablishmentsPluralName => _pluralEstablishmentNames[GroupTypeMode];
        public string PageTitle => string.Concat(GroupUId.HasValue ? "Edit " : "Create ", EntityName);
        public string EntityName => _entityNames.Get(GroupTypeMode);

        public int? GroupUId { get; set; }
        public string GroupName { get; set; }
        public int? GroupTypeId { get; set; }

        public GroupEditorViewModelBase()
        {

        }

        public GroupEditorViewModelBase(GroupModel domainModel)
        {
            GroupUId = domainModel.GroupUID;
            GroupName = domainModel.Name;
            GroupTypeId = domainModel.GroupTypeId;
        }
    }
}