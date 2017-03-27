using Edubase.Services.Enums;
using Edubase.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    public class GroupEditGovernanceViewModel : GroupEditorViewModelBase
    {
        public int? RemovalGID { get; set; }

        public eLookupGovernorRole? GovernorRole { get; set; }
        public int? GID { get; set; }
        public bool ReplaceMode { get; set; }

        public GroupEditGovernanceViewModel(int groupUId, int groupTypeId, string name)
        {
            GroupName = name;
            GroupUId = groupUId;
            GroupTypeId = groupTypeId;
        }

        public GroupEditGovernanceViewModel()
        {

        }
    }
}