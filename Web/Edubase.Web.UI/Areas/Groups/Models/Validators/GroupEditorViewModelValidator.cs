using Edubase.Services.Enums;
using Edubase.Services.Groups;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Validation;
using FluentValidation;
using System.Linq;

namespace Edubase.Web.UI.Areas.Groups.Models.Validators
{
    using Common;
    using Services.Establishments;
    using Services.Security;
    using System.Security.Principal;
    using static GroupEditorViewModel;
    using static GroupEditorViewModelBase;
    using EG = eLookupEstablishmentTypeGroup;
    using GT = eLookupGroupType;
    using VM = GroupEditorViewModel;

    public class GroupEditorViewModelValidator : EdubaseAbstractValidator<VM>
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IGroupReadService _groupReadService;

        public GroupEditorViewModelValidator(IGroupReadService groupReadService, IEstablishmentReadService establishmentReadService, IPrincipal principal, ISecurityService securityService)
        {
            _groupReadService = groupReadService;
            _establishmentReadService = establishmentReadService;
            

            CascadeMode = CascadeMode.StopOnFirstFailure;

            // Searching for an establishment to link....
            When(x => x.Action == ActionLinkedEstablishmentSearch, () =>
            {
                RuleFor(x => x.LinkedEstablishments.LinkedEstablishmentSearch.Urn)
                    .Cascade(CascadeMode.StopOnFirstFailure)

                    .Must(x => x.IsInteger())
                    .WithMessage("Please specify a valid URN")
                    .WithSummaryMessage("The supplied URN is not valid")

                    .Must((model, x) => !model.LinkedEstablishments.Establishments.Select(e => e.Urn).Contains(x.ToInteger().Value))
                    .WithMessage("Link to establishment already exists")
                    .WithSummaryMessage("Link to establishment already exists");
                    
                    //.MustAsync(async (x, ct) => (await _establishmentReadService.GetAsync(x.ToInteger().Value, principal).ConfigureAwait(false)).Success)
                    //.WithMessage("The establishment was not found")
                    //.WithSummaryMessage("The establishment was not found")

                    //.MustAsync(async (x, ct) => (await _establishmentReadService.GetAsync(x.ToInteger().Value, principal).ConfigureAwait(false))
                    //    .GetResult().EstablishmentTypeGroupId == (int)EG.LAMaintainedSchools)
                    //.WithMessage("Establishment is not LA maintained, please select another one.")
                    //.When(m => m.GroupType == GT.Federation || m.GroupType == GT.Trust, ApplyConditionTo.CurrentValidator)

                    //.MustAsync(async (x, ct) => (await _establishmentReadService.GetAsync(x.ToInteger().Value, principal).ConfigureAwait(false))
                    //    .GetResult().EstablishmentTypeGroupId == (int)EG.Academies)
                    //.WithMessage("Establishment is not an academy, please select another one.")
                    //.When(m => m.GroupType == GT.SchoolSponsor, ApplyConditionTo.CurrentValidator)

                    //.MustAsync(async (x, ct) => (await _establishmentReadService.GetAsync(x.ToInteger().Value, principal).ConfigureAwait(false))
                    //    .GetResult().EstablishmentTypeGroupId.Equals((int)EG.ChildrensCentres))
                    //.WithMessage("Establishment is not a children's centre, please select another one.")
                    //.When(m => m.GroupType == GT.ChildrensCentresCollaboration || m.GroupType == GT.ChildrensCentresGroup, ApplyConditionTo.CurrentValidator);
            });

            // Having found an establishment to link, validate the joined date if supplied...
            When(x => x.Action == ActionLinkedEstablishmentAdd, () =>
            {
                RuleFor(x => x.LinkedEstablishments.LinkedEstablishmentSearch.JoinedDate).Must(x => x.IsEmpty() || x.IsValid())
                    .WithMessage("This is not a valid date")
                    .WithSummaryMessage("The Joined Date specified is not valid");
            });

            // Having edited a joined date, validate the date...
            When(x => x.Action == ActionLinkedEstablishmentSave, () =>
            {
                RuleFor(x => x.LinkedEstablishments.Establishments.Single(e => e.EditMode).JoinedDateEditable).Must(x => x.IsEmpty() || x.IsValid())
                    .WithMessage("This is not a valid date")
                    .WithSummaryMessage("The Joined Date specified is not valid");
            });

            // On saving the group record....
            When(x => x.Action == ActionSave, () =>
            {
                When(m => m.GroupTypeMode == eGroupTypeMode.ChildrensCentre, () =>
                {
                    RuleFor(x => x.LocalAuthorityId).NotNull()
                        .WithMessage("This field is mandatory")
                        .WithSummaryMessage("Please select a local authority for the group")
                        .When(x => x.SaveGroupDetail);

                    RuleFor(x => x.LinkedEstablishments.LinkedEstablishmentSearch.Urn)
                        .Must((model, x) => model.LinkedEstablishments.Establishments.Count >= 2)
                        .WithMessage("This group requires two or more centres, please add at least two centres")
                        .When(x => x.SaveEstabLinks);

                    RuleFor(x => x)
                        .Must(x => x.CCLeadCentreUrn.HasValue)
                        .WithMessage("Please select one children's centre to be a group lead")
                        .When(x => x.LinkedEstablishments.Establishments.Count > 0 && x.SaveEstabLinks);
                });

                RuleFor(x => x.GroupTypeId).NotNull().WithMessage("Group Type must be supplied");

                RuleFor(x => x.GroupStatusId)
                    .NotNull()
                    .WithMessage("Select the {0} status", x => x.FieldNamePrefix.ToLower())
                    .When(x => x.SaveGroupDetail);

                RuleFor(x => x.OpenDate)
                    .Must(x => !x.IsEmpty())
                    .WithMessage("{0} missing. Please enter the date", x => x.OpenDateLabel)
                    .When(x => !x.GroupUId.HasValue, ApplyConditionTo.CurrentValidator)
                    .Must(x => x.IsValid() || x.IsEmpty())
                    .WithMessage("{0} is invalid. Please enter a valid date", x => x.OpenDateLabel);
                
                RuleFor(x => x.GroupName)
                    .NotEmpty()
                    .WithMessage("Enter an {0} name", x => x.FieldNamePrefix.ToLower())
                    .When(x => x.SaveGroupDetail)

                    .MustAsync(async (model, name, ct) => !(await _groupReadService.ExistsAsync(securityService.CreateAnonymousPrincipal(), name: name, localAuthorityId: model.LocalAuthorityId.Value, existingGroupUId: model.GroupUId)))
                    .WithMessage("Group name already exists at this authority, please select another name")
                    .When(x => x.GroupTypeMode == eGroupTypeMode.ChildrensCentre && x.LocalAuthorityId.HasValue && x.SaveGroupDetail, ApplyConditionTo.CurrentValidator)

                    .MustAsync(async (model, name, ct) => !(await _groupReadService.ExistsAsync(securityService.CreateAnonymousPrincipal(), name: name, existingGroupUId: model.GroupUId)))
                    .WithMessage("{0} name already exists. Enter a different {0} name", x => x.FieldNamePrefix)
                    .When(x => x.GroupTypeMode == eGroupTypeMode.Sponsor && x.SaveGroupDetail, ApplyConditionTo.CurrentValidator);


                RuleFor(x => x.GroupId)
                    .NotEmpty()
                    .WithMessage("This field is mandatory")
                    .WithSummaryMessage("Please enter a Group ID")
                    .When(x => x.SaveGroupDetail)
                    .MustAsync(async (model, groupId, ct) => !(await _groupReadService.ExistsAsync(securityService.CreateAnonymousPrincipal(), groupId: groupId, existingGroupUId: model.GroupUId)))
                    .WithMessage("Group ID already exists. Enter a different group ID.")
                    .When(x => x.GroupTypeMode.OneOfThese(GT.MultiacademyTrust, GT.SingleacademyTrust, GT.SchoolSponsor) && x.SaveGroupDetail, ApplyConditionTo.AllValidators);

                RuleFor(x => x.GroupName).MustAsync(async (model, name, ct) => !(await _groupReadService.ExistsAsync(securityService.CreateAnonymousPrincipal(), name: name, existingGroupUId: model.GroupUId)))
                    .WithMessage("{0} name already exists, please select another name", m => m.FieldNamePrefix)
                    .When(x => x.GroupTypeMode != eGroupTypeMode.ChildrensCentre && x.GroupTypeMode != eGroupTypeMode.AcademyTrust && x.SaveGroupDetail, ApplyConditionTo.CurrentValidator);
            });
        }

    }
}