using System;
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

    public class GroupEditorViewModelValidator : EdubaseAbstractValidator<GroupEditorViewModel>
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
                    .WithSummaryMessage("Link to establishment already exists")

                    .MustAsync(async (x, ct) =>
                    {
                        return (await _establishmentReadService.GetAsync(x.ToInteger().Value, principal).ConfigureAwait(false)).ReturnValue != null;
                    }).WithMessage("The establishment was not found").WithSummaryMessage("The establishment was not found");
            });

            // Having found an establishment to link, validate the joined date if supplied...
            When(x => x.Action == ActionLinkedEstablishmentAdd, () =>
            {
                RuleFor(x => x.LinkedEstablishments.LinkedEstablishmentSearch.JoinedDate)
                    .Must(x => x.IsValid())
                    .WithMessage("This is not a valid date")
                    .WithSummaryMessage("The Joined Date specified is not valid")

                    .Must((model, joinDate) => VerifyJoinedDate(joinDate.ToDateTime(), model))
                    .WithMessage("The join date you entered is before the {0}'s creation date of {1}. Please enter a later date.", m => m.GroupType, m => m.OpenDate);
            });

            // Having edited a joined date, validate the date...
            When(x => x.Action == ActionLinkedEstablishmentSave, () =>
            {
                RuleFor(x => x.LinkedEstablishments.Establishments.Single(e => e.EditMode).JoinedDateEditable).Must(x => x.IsValid())
                    .WithMessage("This is not a valid date")
                    .WithSummaryMessage("The Joined Date specified is not valid")

                    .Must((model, joinDate) => VerifyJoinedDate(joinDate.ToDateTime(), model))
                    .WithMessage("The join date you entered is before the {0}'s creation date of {1}. Please enter a later date.", m => m.GroupType, m => m.OpenDate);
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
                });

                RuleFor(x => x.GroupTypeId).NotNull().WithMessage("Group Type must be supplied");

                RuleFor(x => x.OpenDate)
                    .Must(x => !x.IsEmpty())
                    .WithMessage("{0} missing. Please enter the date", x => x.OpenDateLabel)
                    .When(x => !x.GroupUId.HasValue, ApplyConditionTo.CurrentValidator)
                    .Must(x => x.IsValid() || x.IsEmpty())
                    .WithMessage("{0} is invalid. Please enter a valid date", x => x.OpenDateLabel);

                RuleFor(x => x.GroupName)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty()
                    .WithMessage("Enter an {0} name", x => x.FieldNamePrefix.ToLower())
                    .When(x => x.SaveGroupDetail)

                    .MustAsync(async (model, name, ct) => !(await _groupReadService.ExistsAsync(securityService.CreateAnonymousPrincipal(), name: name, localAuthorityId: model.LocalAuthorityId.Value, existingGroupUId: model.GroupUId)))
                    .WithMessage("Group name already exists at this authority, please select another name")
                    .When(x => x.GroupTypeMode == eGroupTypeMode.ChildrensCentre && x.LocalAuthorityId.HasValue && x.SaveGroupDetail, ApplyConditionTo.CurrentValidator)

                    .MustAsync(async (model, name, ct) => !(await _groupReadService.ExistsAsync(securityService.CreateAnonymousPrincipal(), name: name, existingGroupUId: model.GroupUId)))
                    .WithMessage("{0} name already exists. Enter a different {0} name", x => x.FieldNamePrefix)
                    .When(x => x.GroupTypeMode == eGroupTypeMode.Sponsor && x.SaveGroupDetail, ApplyConditionTo.CurrentValidator)

                    .MustAsync(async (model, name, ct) => !(await _groupReadService.ExistsAsync(securityService.CreateAnonymousPrincipal(), name: name, existingGroupUId: model.GroupUId)))
                    .WithMessage("{0} name already exists, please select another name", m => m.FieldNamePrefix)
                    .When(x => x.GroupTypeMode != eGroupTypeMode.ChildrensCentre && x.GroupTypeMode != eGroupTypeMode.AcademyTrust && x.SaveGroupDetail, ApplyConditionTo.CurrentValidator);

                RuleFor(x => x.GroupId)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty()
                    .WithMessage("This field is mandatory")
                    .WithSummaryMessage("Please enter a Group ID")
                    .MustAsync(async (model, groupId, ct) => !(await _groupReadService.ExistsAsync(securityService.CreateAnonymousPrincipal(), groupId: groupId, existingGroupUId: model.GroupUId)))
                    .WithMessage("Group ID already exists. Enter a different group ID.")
                    .When(x => x.GroupTypeMode.OneOfThese(eGroupTypeMode.AcademyTrust, eGroupTypeMode.Sponsor) && x.SaveGroupDetail, ApplyConditionTo.AllValidators);

                RuleForEach(x => x.LinkedEstablishments.Establishments)
                    .Must((model, estab) => VerifyJoinedDate(estab.JoinedDateEditable.ToDateTime() ?? estab.JoinedDate, model))
                    .When(x => x.OpenDate.ToDateTime().Value.Date == DateTime.Now.Date)
                    .WithMessage("The join date you entered is before today. Please enter a later date.")
                    .WithSummaryMessage("The join date you entered is before today. Please enter a later date.")

                    .Must((model, estab) => VerifyJoinedDate(estab.JoinedDateEditable.ToDateTime() ?? estab.JoinedDate, model))
                    .When(x => x.OpenDate.ToDateTime().Value.Date != DateTime.Now.Date)
                    .WithMessage("The join date you entered is before the {0}'s creation date of {1}. Please enter a later date.", m => m.GroupType, m => m.OpenDate);

            });
        }

        private bool VerifyJoinedDate(DateTime? joinedDate, GroupEditorViewModel model)
        {
            return model.OpenDate.IsValid() && 
                   model.OpenDate.ToDateTime().HasValue &&
                   joinedDate.HasValue &&
                   joinedDate.Value.Date >= model.OpenDate.ToDateTime().Value.Date;
        }
    }
}