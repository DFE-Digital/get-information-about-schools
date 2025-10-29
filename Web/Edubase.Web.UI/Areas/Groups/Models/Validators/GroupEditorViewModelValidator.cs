using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Groups;
using Edubase.Services.Security;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using FluentValidation;
using static Edubase.Web.UI.Areas.Groups.Models.CreateEdit.GroupEditorViewModel;
using static Edubase.Web.UI.Areas.Groups.Models.CreateEdit.GroupEditorViewModelBase;

namespace Edubase.Web.UI.Areas.Groups.Models.Validators
{
    public class GroupEditorViewModelValidator : AbstractValidator<GroupEditorViewModel>
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IGroupReadService _groupReadService;
        private EstablishmentModel _matchedEstablishment;

        public GroupEditorViewModelValidator(
            IGroupReadService groupReadService,
            IEstablishmentReadService establishmentReadService,
            IPrincipal principal,
            ISecurityService securityService)
        {
            _groupReadService = groupReadService;
            _establishmentReadService = establishmentReadService;

            // Establishment search validation
            When(x => x.Action == ActionLinkedEstablishmentSearch, () =>
            {
                RuleFor(x => x.LinkedEstablishments.LinkedEstablishmentSearch.Urn)
                    .NotEmpty().WithMessage("Please enter a valid URN")
                    .Must(x => x.IsInteger()).WithMessage("The supplied URN is not valid")
                    .Must((model, urnText) =>
                    {
                        var urn = urnText.ToInteger();
                        return urn.HasValue && !model.LinkedEstablishments.Establishments.Any(e => e.Urn == urn.Value);
                    }).WithMessage("This establishment is already in this group. Please enter a different URN")
                    .MustAsync(async (urnText, ct) =>
                    {
                        var est = await GetOrFetchMatchedEstablishment(_establishmentReadService, principal, urnText);
                        return est != null;
                    }).WithMessage("The establishment was not found")
                    .MustAsync(async (model, urnText, ct) =>
                    {
                        var est = await GetOrFetchMatchedEstablishment(_establishmentReadService, principal, urnText);
                        return model.GroupTypeMode != eGroupTypeMode.ChildrensCentre || est?.EstablishmentTypeGroupId == 4;
                    }).WithMessage("Enter a URN for a children's centre or children's centre linked site");
            });

            // Joined date validation (add)
            When(x => x.Action == ActionLinkedEstablishmentAdd, () =>
            {
                RuleFor(x => x.LinkedEstablishments.LinkedEstablishmentSearch.JoinedDate)
                    .Must(x => x.IsValid()).WithMessage("This is not a valid date")
                    .Must((model, date) => VerifyJoinedDate(date.ToDateTime(), model))
                    .WithMessage(x => GenerateJoinDateBeforeOpenDateMessage(x));
            });

            // Joined date validation (edit)
            When(x => x.Action == ActionLinkedEstablishmentSave, () =>
            {
                RuleFor(x => x.LinkedEstablishments.Establishments.Single(e => e.EditMode).JoinedDateEditable)
                    .Must(x => x.IsValid()).WithMessage("This is not a valid date")
                    .Must((model, date) => VerifyJoinedDate(date.ToDateTime(), model))
                    .WithMessage(x => GenerateJoinDateBeforeOpenDateMessage(x));
            });

            // Save page validations
            When(x => x.Action == ActionSave || x.Action == ActionDetails, () =>
            {
                When(m => m.GroupTypeMode == eGroupTypeMode.ChildrensCentre, () =>
                {
                    RuleFor(x => x.LocalAuthorityId)
                        .NotNull().WithMessage("This field is mandatory")
                        .When(x => x.SaveGroupDetail);
                });

                RuleFor(x => x.GroupTypeId)
                    .NotNull().WithMessage("Group Type must be supplied");

                RuleFor(x => x.OpenDate)
                    .Must(x => !x.IsEmpty()).WithMessage(x => $"{x.OpenDateLabel} missing. Please enter the date")
                    .When(x => !x.GroupUId.HasValue)
                    .Must(x => x.IsValid() || x.IsEmpty()).WithMessage(x => $"{x.OpenDateLabel} is invalid. Please enter a valid date");

                When(x => x.CloseAndMarkAsCreatedInError && x.StatusId == (int) eLookupGroupStatus.Closed, () =>
                {
                    RuleFor(x => x.CloseAndMarkAsCreatedInError)
                        .Must(x => false)
                        .WithMessage("Please select 'created in error' or enter a close date, but not both");

                    RuleFor(x => x.ClosedDate)
                        .Must(x => false)
                        .WithMessage("Please enter a close date or select 'created in error', but not both");
                });

                When(x => x.CanUserEditClosedDate &&
                          x.GroupType == eLookupGroupType.MultiacademyTrust &&
                          x.OriginalStatusId != (int) eLookupGroupStatus.Closed &&
                          x.StatusId == (int) eLookupGroupStatus.Closed &&
                          x.SaveGroupDetail, () =>
                          {
                              RuleFor(x => x.ClosedDate)
                                  .Must(x => !x.IsEmpty()).WithMessage("Please enter a date for the closure of this multi-academy trust")
                                  .Must(x => x.IsValid() || x.IsEmpty()).WithMessage("Closed date is invalid. Please enter a valid date.");
                          });

                When(x => x.GroupType == eLookupGroupType.SecureSingleAcademyTrust &&
                          x.OriginalStatusId != (int) eLookupGroupStatus.Closed &&
                          x.StatusId == (int) eLookupGroupStatus.Closed &&
                          x.SaveGroupDetail, () =>
                          {
                              RuleFor(x => x.ClosedDate)
                                  .Must(x => !x.IsEmpty()).WithMessage("Please enter a date for the closure of this secure single-academy trust")
                                  .Must(x => x.IsValid() || x.IsEmpty()).WithMessage("Closed date is invalid. Please enter a valid date.");
                          });

                RuleFor(x => x.GroupName)
                    .NotEmpty().WithMessage(x => $"Please enter the {x.GroupTypeLabelPrefix.ToLower()} name")
                    .When(x => x.SaveGroupDetail);

                When(x => x.GroupType != eLookupGroupType.SecureSingleAcademyTrust, () =>
                {
                    RuleFor(x => x.GroupId)
                        .NotEmpty().WithMessage("Please enter a Group ID")
                        .MustAsync(async (model, groupId, ct) =>
                        {
                            return !await _groupReadService.ExistsAsync(
                                securityService.CreateAnonymousPrincipal(),
                                groupId: groupId,
                                existingGroupUId: model.GroupUId);
                        })
                        .WithMessage("Group ID already exists. Enter a different group ID.")
                        .When(x => x.GroupTypeMode.OneOfThese(eGroupTypeMode.AcademyTrust, eGroupTypeMode.Sponsor) && x.SaveGroupDetail);
                });

                When(x => x.OpenDate.ToDateTime().HasValue, () =>
                {
                    RuleForEach(x => x.LinkedEstablishments.Establishments)
                        .Must((model, estab) => VerifyJoinedDate(estab.JoinedDateEditable.ToDateTime() ?? estab.JoinedDate, model))
                        .When(x => x.OpenDate.ToDateTime().Value.Date == DateTime.Now.Date)
                        .WithMessage("The join date you entered is before today. Please enter a later date.")
                        .Must((model, estab) => VerifyJoinedDate(estab.JoinedDateEditable.ToDateTime() ?? estab.JoinedDate, model))
                        .When(x => x.OpenDate.ToDateTime().Value.Date != DateTime.Now.Date)
                        .WithMessage(x => GenerateJoinDateBeforeOpenDateMessage(x));
                });
            });

            // Minimum centres for Children's Centre group
            When(x => x.Action == ActionSave && x.ActionName == eChildrensCentreActions.Step3, () =>
            {
                When(m => m.GroupTypeMode == eGroupTypeMode.ChildrensCentre, () =>
                {
                    RuleFor(x => x.LinkedEstablishments.Establishments)
                        .Must(x => x.Count >= 2)
                        .WithMessage("Add more centres to the group");
                });
            });
        }

        private async Task<EstablishmentModel> GetOrFetchMatchedEstablishment(IEstablishmentReadService establishmentReadService, IPrincipal principal, string urnSearchText)
        {
            if (_matchedEstablishment == null)
            {
                _matchedEstablishment = (await establishmentReadService.GetAsync(urnSearchText.ToInteger().Value, principal).ConfigureAwait(false))
                    .ReturnValue;
            }

            return _matchedEstablishment;
        }

        private bool VerifyJoinedDate(DateTime? joinedDate, GroupEditorViewModel model)
        {
            return model.OpenDate.IsValid() &&
                   model.OpenDate.ToDateTime().HasValue &&
                   joinedDate.HasValue &&
                   joinedDate.Value.Date >= model.OpenDate.ToDateTime().Value.Date;
        }

        private string GenerateJoinDateBeforeOpenDateMessage(GroupEditorViewModel viewModel)
        {
            return $"An establishment can only join the {viewModel.GroupTypeLabelPrefix.ToLower()} " +
                   $"on the date the {viewModel.GroupTypeLabelPrefix.ToLower()} is opened or a date afterwards. " +
                   $"The {viewModel.GroupTypeLabelPrefix.ToLower()} opened on {viewModel.OpenDate}. " +
                   $"A valid joining date must be entered.";
        }
    }
}
