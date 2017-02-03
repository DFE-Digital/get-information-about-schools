using Edubase.Services.Groups;
using FluentValidation;
using FluentValidation.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation.Results;
using System.Web.Mvc;
using Edubase.Web.UI.Validation;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using MoreLinq;
using Edubase.Services.Enums;

namespace Edubase.Web.UI.Areas.Groups.Models.Validators
{
    using Common;
    using Services.Establishments;
    using Services.Security;
    using VM = GroupEditorViewModel;
    using GT = eLookupGroupType;
    using ET = eLookupEstablishmentType;
    using EG = eLookupEstablishmentTypeGroup;

    public class GroupEditorViewModelValidator : EdubaseAbstractValidator<VM>
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IGroupReadService _groupReadService;
        private readonly ISecurityService _securityService;

        public GroupEditorViewModelValidator(IGroupReadService groupReadService, IEstablishmentReadService establishmentReadService, ISecurityService securityService)
        {
            _groupReadService = groupReadService;
            _establishmentReadService = establishmentReadService;
            _securityService = securityService;
            var principal = _securityService.CreateSystemPrincipal();

            CascadeMode = CascadeMode.StopOnFirstFailure;

            // Searching for an establishment to link....
            When(x => x.Action == VM.ActionLinkedEstablishmentSearch, () =>
            {
                RuleFor(x => x.LinkedEstablishments.LinkedEstablishmentSearch.Urn)
                    .Cascade(CascadeMode.StopOnFirstFailure)

                    .Must(x => x.IsInteger())
                    .WithMessage("Please specify a valid URN")
                    .WithSummaryMessage("The supplied URN is not valid")
                    
                    .Must((model, x) => !model.LinkedEstablishments.Establishments.Select(e => e.Urn).Contains(x.ToInteger().Value))
                    .WithMessage("Link to establishment already exists")
                    .WithSummaryMessage("Link to establishment already exists")
                    
                    .MustAsync(async (x, ct) => (await _establishmentReadService.GetAsync(x.ToInteger().Value, principal).ConfigureAwait(false)).Success)
                    .WithMessage("The establishment was not found")
                    .WithSummaryMessage("The establishment was not found")

                    .MustAsync(async (x, ct) => (await _establishmentReadService.GetAsync(x.ToInteger().Value, principal).ConfigureAwait(false))
                        .GetResult().EstablishmentTypeGroupId == (int)EG.LAMaintainedSchools)
                    .WithMessage("Establishment is not LA maintained, please select another one.")
                    .When(m => m.GroupType == GT.Federation || m.GroupType == GT.Trust, ApplyConditionTo.CurrentValidator)

                    .MustAsync(async (x, ct) => (await _establishmentReadService.GetAsync(x.ToInteger().Value, principal).ConfigureAwait(false))
                        .GetResult().EstablishmentTypeGroupId.Equals((int)EG.ChildrensCentres))
                    .WithMessage("Establishment is not a children's centre, please select another one.")
                    .When(m => m.GroupType == GT.ChildrensCentresCollaboration || m.GroupType == GT.ChildrensCentresGroup, ApplyConditionTo.CurrentValidator);
            });

            // Having found an establishment to link, validate the joined date if supplied...
            When(x => x.Action == VM.ActionLinkedEstablishmentAdd, () =>
            {
                RuleFor(x => x.LinkedEstablishments.LinkedEstablishmentSearch.JoinedDate).Must(x => x.IsEmpty() || x.IsValid())
                    .WithMessage("This is not a valid date")
                    .WithSummaryMessage("The Joined Date specified is not valid");
            });

            // Having edited a joined date, validate the date...
            When(x => x.Action == VM.ActionLinkedEstablishmentSave, () =>
            {
                RuleFor(x => x.LinkedEstablishments.Establishments.Single(e => e.EditMode).JoinedDateEditable).Must(x => x.IsEmpty() || x.IsValid())
                    .WithMessage("This is not a valid date")
                    .WithSummaryMessage("The Joined Date specified is not valid");
            });

            // On saving the group record....
            When(x => x.Action == VM.ActionSave, () =>
            {
                When(m => m.GroupTypeMode == VM.eGroupTypeMode.ChildrensCentre, () =>
                {
                    RuleFor(x => x.LocalAuthorityId).NotNull()
                        .WithMessage("This field is mandatory")
                        .WithSummaryMessage("Please select a local authority for the group");

                    RuleFor(x => x.LinkedEstablishments.LinkedEstablishmentSearch.Urn)
                        .Must((model, x) => model.LinkedEstablishments.Establishments.Count >= 2)
                        .WithMessage("This group requires two or more centres, please add at least two centres");

                    RuleFor(x => x)
                        .Must(x => x.CCLeadCentreUrn.HasValue)
                        .WithMessage("Please select one children's centre to be a group lead")
                        .When(x => x.LinkedEstablishments.Establishments.Count > 0);
                });

                RuleFor(x => x.GroupTypeId).NotNull().WithMessage("Group Type must be supplied");

                RuleFor(x => x.GroupStatusId).NotNull().WithMessage("This is a mandatory field").WithSummaryMessage("Status must be set");

                RuleFor(x => x.OpenDate)
                    .Must(x => x.IsEmpty() || x.IsValid())
                    .WithMessage("Open date is invalid")
                    .Must(x => x.IsNotEmpty() && x.IsValid())
                    .WithMessage("{0} is a mandatory field", m => m.OpenDateLabel)
                    .When(x => !x.GroupUID.HasValue, ApplyConditionTo.CurrentValidator);

                RuleFor(x => x.Name)
                    .NotEmpty()
                    .WithMessage("This field is mandatory").WithSummaryMessage("Please enter a name for the group")
                    .MustAsync(async (model, name, ct) => !(await _groupReadService.ExistsAsync(name, model.LocalAuthorityId.Value, model.GroupUID)))
                    .WithMessage("Group name already exists at this authority, please select another name")
                    .When(x => x.GroupTypeMode == VM.eGroupTypeMode.ChildrensCentre && x.LocalAuthorityId.HasValue, ApplyConditionTo.CurrentValidator);

                RuleFor(x => x.Name).MustAsync(async (model, name, ct) => !(await _groupReadService.ExistsAsync(name, existingGroupUId: model.GroupUID)))
                    .WithMessage("{0} name already exists, please select another name", m => m.FieldNamePrefix)
                    .When(x => x.GroupTypeMode != VM.eGroupTypeMode.ChildrensCentre && x.GroupTypeMode != VM.eGroupTypeMode.AcademyTrust, ApplyConditionTo.CurrentValidator);
            });
        }

    }
}