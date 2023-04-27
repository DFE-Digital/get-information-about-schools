using System.Security.Principal;
using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Services.Security;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Helpers;

namespace Edubase.Web.UI.Areas.Groups.ViewRulesHandlers
{
    public static class GroupEditorViewModelRulesHandler
    {
        public static GroupEditorViewModel SetEditPermissions(GroupEditorViewModel viewModel, IPrincipal user)
        {
            viewModel.CanUserCloseAndMarkAsCreatedInError = UserCanCloseAndMarkAsCreatedInError(viewModel, user);

            viewModel.IsLocalAuthorityEditable = IsLocalAuthorityEditable(viewModel, user);

            var userCanEditClosedDateAndStatus = UserCanEditClosedDateAndStatus(viewModel, user);
            if (userCanEditClosedDateAndStatus)
            {
                viewModel.CanUserEditClosedDate = true;
                viewModel.CanUserEditStatus = true;
            }

            viewModel.CanUserEditUkprn = UserCanEditUkprn(viewModel, user);

            return viewModel;
        }

        public static bool IsLocalAuthorityEditable(GroupEditorViewModel viewModel, IPrincipal user)
        {
            return viewModel.GroupTypeId.OneOfThese(eLookupGroupType.ChildrensCentresCollaboration, eLookupGroupType.ChildrensCentresGroup)
                                                 && viewModel.LinkedEstablishments.Establishments.Count == 0
                                                 && user.InRole(AuthorizedRoles.IsAdmin);
        }

        public static bool UserCanCloseAndMarkAsCreatedInError(GroupEditorViewModel viewModel, IPrincipal user)
        {
            return viewModel.GroupType.OneOfThese(eLookupGroupType.MultiacademyTrust,
                                                eLookupGroupType.SingleacademyTrust,
                                                eLookupGroupType.SchoolSponsor,
                                                eLookupGroupType.Federation,
                                                eLookupGroupType.SecureSingleAcademyTrust)
                && !viewModel.StatusId.OneOfThese(eLookupGroupStatus.CreatedInError, eLookupGroupStatus.Closed)
                && user.InRole(AuthorizedRoles.IsAdmin);
        }

        public static bool UserCanEditClosedDateAndStatus(GroupEditorViewModel viewModel, IPrincipal user)
        {
            var result = false;
            if (viewModel.GroupType.OneOfThese(eLookupGroupType.MultiacademyTrust, eLookupGroupType.SingleacademyTrust)
                && user.InRole(AuthorizedRoles.CanBulkAssociateEstabs2Groups))
            {
                result = true;
            }
            else if (viewModel.GroupType == eLookupGroupType.SecureSingleAcademyTrust
                && user.InRole(AuthorizedRoles.IsAdmin))
            {
                result = true;
            }
            return result;
        }

        public static bool UserCanEditUkprn(GroupEditorViewModel viewModel, IPrincipal user)
        {
            var result = false;
            if (viewModel.GroupType.OneOfThese(eLookupGroupType.MultiacademyTrust, eLookupGroupType.SingleacademyTrust)
                && user.InRole(AuthorizedRoles.IsAdmin))
            {
                result = true;
            }
            else if (viewModel.GroupType == eLookupGroupType.SecureSingleAcademyTrust
                && user.InRole(EdubaseRoles.ROLE_BACKOFFICE + "," + EdubaseRoles.UKRLP))
            {
                result = true;
            }
            return result;
        }

        public static bool MustShowChangesReviewScreen(GroupEditorViewModel viewModel, IPrincipal user)
        {
            var result = false;
            if (!viewModel.ChangesAcknowledged)
            {
                result = viewModel.GroupTypeId.OneOfThese(eLookupGroupType.SingleacademyTrust, eLookupGroupType.MultiacademyTrust)
                        || (viewModel.GroupType == eLookupGroupType.SecureSingleAcademyTrust && user.IsInRole(EdubaseRoles.UKRLP));
            }
            return result;
        }
    }
}
