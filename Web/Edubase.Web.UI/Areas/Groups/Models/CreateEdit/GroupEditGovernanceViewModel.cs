using Edubase.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    public class GroupEditGovernanceViewModel : GroupEditorViewModelBase
    {
        public GovernorsGridViewModel GovernorsDetails { get; set; }
        public List<LookupItemViewModel> GovernorRoles { get; internal set; }

        public GroupEditGovernanceViewModel(int groupUId, int groupTypeId, string name)
        {
            GroupName = name;
            GroupUId = groupUId;
            GroupTypeId = groupTypeId;
        }
    }
}