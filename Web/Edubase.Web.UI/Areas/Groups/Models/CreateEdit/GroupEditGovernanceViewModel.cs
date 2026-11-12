using Edubase.Services.Enums;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    public class GroupEditGovernanceViewModel : GroupEditorViewModelBase
    {
        public int? RemovalGID { get; set; }

        public eLookupGovernorRole? GovernorRole { get; set; }
        public int? GID { get; set; }
        public bool ReplaceMode { get; set; }

        public GroupEditGovernanceViewModel(int groupUId, int groupTypeId, string name, string typeName)
        {
            GroupName = name;
            GroupUId = groupUId;
            GroupTypeId = groupTypeId;
            GroupTypeName = typeName;
        }

        public GroupEditGovernanceViewModel()
        {

        }
    }
}
