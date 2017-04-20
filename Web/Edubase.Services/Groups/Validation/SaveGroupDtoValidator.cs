using Edubase.Services.Enums;
using Edubase.Services.Groups.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Groups.Validation
{
    using Common;
    using Establishments;
    using Security;
    using System.Security.Principal;
    using GT = eLookupGroupType;

    internal class SaveGroupDtoValidator : AbstractValidator<SaveGroupDto>
    {
        IPrincipal _principal;
        IEstablishmentReadService _establishmentReadService;
        ISecurityService _securityService;
        IGroupReadService _groupReadService;

        public SaveGroupDtoValidator(IPrincipal principal, 
            IEstablishmentReadService establishmentReadService, 
            ISecurityService securityService,
            IGroupReadService groupReadService)
        {
            _principal = principal;
            _establishmentReadService = establishmentReadService;
            _securityService = securityService;
            _groupReadService = groupReadService;

            CreateRules();
        }

        public void CreateRules()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            var permission = _securityService.GetCreateGroupPermission(_principal);

            RuleFor(x => x.Group.LocalAuthorityId)
                .Must(x => permission.IsLAAllowed(x))
                .WithMessage("Permission denied creating/editing a children's centre group for the local authority (LocalAuthorityId) supplied")
                .When(x => x.Group.GroupTypeId.OneOfThese(GT.ChildrensCentresCollaboration, GT.ChildrensCentresGroup));

            RuleFor(x => x.Group.Name)
                .NotEmpty()
                .WithMessage("The Group Name must not be empty")
                .MustAsync(async (model, name, ct) => !await _groupReadService.ExistsAsync(_securityService.CreateAnonymousPrincipal(), name, model.Group.LocalAuthorityId, model.Group.GroupUID))
                .WithMessage("The specified group name already exists")
                .When(x => x.Group.GroupTypeId != (int)GT.MultiacademyTrust && x.Group.GroupTypeId != (int)GT.SingleacademyTrust, ApplyConditionTo.CurrentValidator); // user has no control over academy trust names

            RuleFor(x => x.Group.GroupId)
                .NotEmpty()
                .WithMessage("The Group ID must not be empty")
                .MustAsync(async (model, groupId, ct) => !await _groupReadService.ExistsAsync(_securityService.CreateAnonymousPrincipal(), groupId, model.Group.GroupUID))
                .WithMessage("The specified group id already exists")
                .When(x => x.Group.GroupTypeId.OneOfThese(GT.MultiacademyTrust, GT.SingleacademyTrust, GT.SchoolSponsor), ApplyConditionTo.AllValidators); // user has no control over academy trust names

            RuleFor(x => x.Group.GroupTypeId).NotEmpty().WithMessage("The Group Type must not be empty");
            RuleFor(x => x.Group.StatusId).NotEmpty().WithMessage("The Group Status must not be empty");

            RuleFor(x => x.Group.ManagerEmailAddress).Empty().When(x => x.Group.GroupTypeId != (int)GT.ChildrensCentresGroup)
                .WithMessage("'ManagerEmailAddress' should only be supplied when the group type is ChildrensCentresGroup");

            RuleFor(x => x.LinkedEstablishments).Must((model, x) => !x.GroupBy(e => e.EstablishmentUrn).Where(g => g.Count() > 1).Any())
                .WithMessage("There are duplicate linked establishments defined")
                .When(x => x.LinkedEstablishments != null && x.LinkedEstablishments.Count >= 2);
                             
            // When an MAT/SAT group type
            When(x => x.Group.GroupTypeId.OneOfThese(GT.MultiacademyTrust, GT.SingleacademyTrust), () =>
            {
                RuleFor(x => x.Group.Address.ToString())
                    .NotEmpty().WithMessage("The Address field should be supplied for types: MultiAcademyTrust and SingleAcademyTrust.")
                    .When(x => x.IsNewEntity);

                RuleFor(x => x.Group.CompaniesHouseNumber)
                    .NotEmpty().WithMessage("The CompaniesHouseNumber field should be supplied for types: MultiAcademyTrust and SingleAcademyTrust.");

                RuleFor(x => x.Group.ClosedDate).Empty().WithMessage("'ClosedDate' must not be supplied for group types: MultiAcademyTrust and SingleAcademyTrust");
            });

            // When creating a new group record
            When(x => x.IsNewEntity, () =>
            {
                RuleFor(x => x.LinkedEstablishments).Must(x => x == null || x.Count == 0)
                    .WithMessage("For groups of type 'ChildrensCentresCollaboration' or 'ChildrensCentresGroup' no linked establishments must be defined at the point of creation of the entity.")
                    .When(x => !x.Group.GroupTypeId.OneOfThese(GT.ChildrensCentresCollaboration, GT.ChildrensCentresGroup));

                RuleFor(x => x.Group.OpenDate).NotNull().WithMessage("The open date / incorporated date must be supplied for new groups.");
            });

            // When group type is a trust or federation
            When(x => x.Group.GroupTypeId.OneOfThese(GT.Federation, GT.Trust), () =>
            {
                RuleForEach(x => x.LinkedEstablishments)
                    .MustAsync(async (m, e, ct) => (await _establishmentReadService.GetAsync(e.EstablishmentUrn, _principal))
                    .GetResult().EstablishmentTypeGroupId.OneOfThese(eLookupEstablishmentTypeGroup.LAMaintainedSchools))
                    .WithMessage("The linked establishments' type groups must be LAMaintainedSchools");
            });

            // When group type is a sponsor
            When(x => x.Group.GroupTypeId.OneOfThese(GT.SchoolSponsor), () =>
            {
                RuleForEach(x => x.LinkedEstablishments)
                    .MustAsync(async (m, e, ct) => (await _establishmentReadService.GetAsync(e.EstablishmentUrn, _principal))
                    .GetResult().EstablishmentTypeGroupId.OneOfThese(eLookupEstablishmentTypeGroup.Academies))
                    .WithMessage("The linked establishments' type groups must be Academies");
            });

            // A children's centre group
            When(x => x.Group.GroupTypeId.OneOfThese(GT.ChildrensCentresCollaboration, GT.ChildrensCentresGroup), () =>
            {
                RuleFor(x => x.LinkedEstablishments)
                    .NotNull().WithMessage("LinkedEstablishments cannot be empty / null")
                    .When(x => !x.GetGroupUId().HasValue, ApplyConditionTo.CurrentValidator)
                    .Must(x => x.Count >= 2).WithMessage("Groups of type 'ChildrensCentresCollaboration' or 'ChildrensCentresGroup' require at least 2 centres / linked establishments.")
                    .When(x => x.LinkedEstablishments != null, ApplyConditionTo.CurrentValidator)
                    .Must(x => x.Any(e => e.CCIsLeadCentre)).WithMessage("Groups of type 'ChildrensCentresCollaboration' or 'ChildrensCentresGroup' require at least one linked establishment to be flagged as a lead centre with the CCIsLeadCentre property")
                    .When(x => x.LinkedEstablishments != null, ApplyConditionTo.CurrentValidator);

                RuleForEach(x => x.LinkedEstablishments)
                    .MustAsync(async (m, e, ct) => (await _establishmentReadService.GetAsync(e.EstablishmentUrn, _principal))
                    .GetResult().EstablishmentTypeGroupId.OneOfThese(eLookupEstablishmentTypeGroup.ChildrensCentres))
                    .WithMessage("The linked establishments' type groups must be ChildrensCentres")
                    .When(x => x.LinkedEstablishments != null, ApplyConditionTo.CurrentValidator); ;

                RuleFor(x => x.Group.LocalAuthorityId).NotNull()
                    .WithMessage("LocalAuthorityId must be supplied for group types ChildrensCentresCollaboration and ChildrensCentresGroup")
                    .When(x => x.Group != null);
            });

            // Not a chilren's centre group
            When(x => x.Group != null && !x.Group.GroupTypeId.OneOfThese(GT.ChildrensCentresCollaboration, GT.ChildrensCentresGroup), () =>
            {
                RuleFor(x => x.Group.LocalAuthorityId).Null().WithMessage("LocalAuthorityId can only be supplied for group types ChildrensCentresCollaboration and ChildrensCentresGroup");
            });
        }
    }
}
