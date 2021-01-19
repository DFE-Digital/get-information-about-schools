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
                    .WithMessage("Please enter a valid URN")
                    .WithSummaryMessage("The supplied URN is not valid")

                    .Must((model, x) => !model.LinkedEstablishments.Establishments.Select(e => e.Urn).Contains(x.ToInteger().Value))
                    .WithMessage("This establishment is already in this group. Please enter a different URN")
                    .WithSummaryMessage("This establishment is already in this group. Please enter a different URN")

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
                    .WithMessage(x => $"The join date you entered is before the {x.GroupType}'s open date of {x.OpenDate}. Please enter a later date.");
            });

            // Having edited a joined date, validate the date...
            When(x => x.Action == ActionLinkedEstablishmentSave, () =>
            {
                RuleFor(x => x.LinkedEstablishments.Establishments.Single(e => e.EditMode).JoinedDateEditable).Must(x => x.IsValid())
                    .WithMessage("This is not a valid date")
                    .WithSummaryMessage("The Joined Date specified is not valid")

                    .Must((model, joinDate) => VerifyJoinedDate(joinDate.ToDateTime(), model))
                    .WithMessage(x => $"The join date you entered is before the {x.GroupType}'s open date of {x.OpenDate}. Please enter a later date.");
            });

            // On getting to the save page....
            When(x => x.Action == ActionSave || x.Action == ActionDetails, () =>
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
                    .WithMessage(x => $"{x.OpenDateLabel} missing. Please enter the date")
                    .When(x => !x.GroupUId.HasValue, ApplyConditionTo.CurrentValidator)
                    .Must(x => x.IsValid() || x.IsEmpty())
                    .WithMessage(x => $"{x.OpenDateLabel} is invalid. Please enter a valid date");

                When(x => x.CanUserEditClosedDate
                    && x.GroupType == eLookupGroupType.MultiacademyTrust
                    && x.OriginalStatusId != (int) eLookupGroupStatus.Closed
                    && x.StatusId == (int) eLookupGroupStatus.Closed
                    && x.SaveGroupDetail, () =>
                    {
                        RuleFor(x => x.ClosedDate)
                        .Must(x => !x.IsEmpty())
                        .WithMessage("Please enter a date for the closure of this multi-academy trust")
                        .Must(x => x.IsValid() || x.IsEmpty())
                        .WithMessage("Closed date is invalid. Please enter a valid date.");
                    });

                RuleFor(x => x.GroupName)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty()
                    .WithMessage(x => $"Please enter the {x.FieldNamePrefix.ToLower()} name")
                    .When(x => x.SaveGroupDetail);

                RuleFor(x => x.GroupId)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty()
                    .WithMessage("Please enter a Group ID")
                    .WithSummaryMessage("Please enter a Group ID")
                    .MustAsync(async (model, groupId, ct) => !(await _groupReadService.ExistsAsync(securityService.CreateAnonymousPrincipal(), groupId: groupId, existingGroupUId: model.GroupUId)))
                    .WithMessage("Group ID already exists. Enter a different group ID.")
                    .When(x => x.GroupTypeMode.OneOfThese(eGroupTypeMode.AcademyTrust, eGroupTypeMode.Sponsor) && x.SaveGroupDetail, ApplyConditionTo.AllValidators);

                When(x => x.OpenDate.ToDateTime().HasValue, () =>
                {
                    RuleForEach(x => x.LinkedEstablishments.Establishments)
                        .Must((model, estab) => VerifyJoinedDate(estab.JoinedDateEditable.ToDateTime() ?? estab.JoinedDate, model))
                        .When(x => x.OpenDate.ToDateTime().GetValueOrDefault().Date == DateTime.Now.Date)
                        .WithMessage("The join date you entered is before today. Please enter a later date.")
                        .WithSummaryMessage("The join date you entered is before today. Please enter a later date.")

                        .Must((model, estab) => VerifyJoinedDate(estab.JoinedDateEditable.ToDateTime() ?? estab.JoinedDate, model))
                        .When(x => x.OpenDate.ToDateTime().GetValueOrDefault().Date != DateTime.Now.Date)
                        .WithMessage(x => $"The join date you entered is before the {x.GroupType}'s open date of {x.OpenDate}. Please enter a later date.");
                });
            });

            // Specific addition only triggered upon Saving ChildrensCentres
            When(x => x.Action == ActionSave, () =>
            {
                When(m => m.GroupTypeMode == eGroupTypeMode.ChildrensCentre, () =>
                {
                    RuleFor(x => x.LinkedEstablishments.Establishments)
                        .Must(x => x.Count > 2)
                        .WithMessage("Add more centres to the group")
                        .WithSummaryMessage("You need to add at least two centres");
                });
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
