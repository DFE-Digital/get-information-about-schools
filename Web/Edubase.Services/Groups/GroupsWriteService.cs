using Edubase.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Edubase.Data.DbContext;
using Edubase.Services.Groups.Models;
using System.Security.Principal;
using Edubase.Services.Enums;

namespace Edubase.Services.Groups
{
    using Common;
    using Data.Repositories.Groups.Abstract;
    using Domain;
    using Establishments;
    using Exceptions;
    using Security;
    using Validation;
    using GT = eLookupGroupType;

    public class GroupsWriteService : IGroupsWriteService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IEstablishmentReadService _establishmentsReadService;
        private readonly IGroupReadService _groupReadService;
        private readonly ISecurityService _securityService;
        private readonly ICachedGroupReadRepository _groupRepository;
        private readonly ICachedEstablishmentGroupReadRepository _cachedEstablishmentGroupReadRepository;

        public GroupsWriteService(
            IApplicationDbContext dbContext, 
            IEstablishmentReadService establishmentsReadService, 
            ISecurityService securityService, 
            IGroupReadService groupReadService, 
            ICachedGroupReadRepository groupRepository,
            ICachedEstablishmentGroupReadRepository cachedEstablishmentGroupReadRepository)
        {
            _dbContext = dbContext;
            _establishmentsReadService = establishmentsReadService;
            _securityService = securityService;
            _groupReadService = groupReadService;
            _groupRepository = groupRepository;
            _cachedEstablishmentGroupReadRepository = cachedEstablishmentGroupReadRepository;
        }

        public async Task<int> SaveAsync(SaveGroupDto dto, IPrincipal principal)
        {
            var newRecord = dto.IsNewEntity;

            if (dto.Group == null && dto.GetGroupUId().HasValue) dto.Group = (await _groupReadService.GetAsync(dto.GetGroupUId().Value, _securityService.CreateSystemPrincipal())).GetResult();
            else if (dto.Group == null && !dto.GetGroupUId().HasValue) throw new InvalidOperationException($"The Group object is null and no UId was supplied.  I cannot work with this.");

            var validator = new SaveGroupDtoValidator(principal, _establishmentsReadService, _securityService, _groupReadService);
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid) throw new EdubaseException("Validation errors: \r\n" + string.Join(Environment.NewLine, validationResult.Errors.Select(x => x.ErrorMessage)));
            else
            {
                dto.Group.EstablishmentCount = (dto.LinkedEstablishments?.Count).GetValueOrDefault();

                var changes = Enumerable.Empty<ChangeDescriptorDto>();
                if (!dto.IsNewEntity) changes = await _groupReadService.GetModelChangesAsync(dto.Group);

                if(dto.Group.GroupTypeId == (int)GT.ChildrensCentresGroup && dto.LinkedEstablishments != null)
                {
                    var leadCentreUrn = dto.LinkedEstablishments.Single(x => x.CCIsLeadCentre).EstablishmentUrn;
                    var e = (await _establishmentsReadService.GetAsync(leadCentreUrn, principal)).GetResult();
                    dto.Group.Address = e.GetAddress();
                }
                
                var dataModel = !dto.IsNewEntity ? _dbContext.Groups.SingleOrThrow(x => x.GroupUID == dto.Group.GroupUID) : new GroupCollection();

                dataModel.Address = dto.Group.Address;
                dataModel.ClosedDate = dto.Group.ClosedDate;
                dataModel.CompaniesHouseNumber = dto.Group.CompaniesHouseNumber;
                dataModel.EstablishmentCount = (dto.LinkedEstablishments?.Count).GetValueOrDefault();
                dataModel.GroupId = dto.Group.GroupId;
                dataModel.GroupTypeId = dto.Group.GroupTypeId;
                dataModel.LocalAuthorityId = dto.Group.LocalAuthorityId;
                dataModel.ManagerEmailAddress = dto.Group.ManagerEmailAddress;
                dataModel.Name = dto.Group.Name;
                dataModel.OpenDate = dto.Group.OpenDate;
                dataModel.StatusId = dto.Group.StatusId;

                if (dto.IsNewEntity) _dbContext.Groups.Add(dataModel);

                if (dto.LinkedEstablishments != null)
                {
                    foreach (var e in dto.LinkedEstablishments)
                    {
                        var linkedEstabDataModel = e.Id.HasValue ? _dbContext.EstablishmentGroups.SingleOrThrow(x => x.Id == e.Id) : new EstablishmentGroup();
                        linkedEstabDataModel.EstablishmentUrn = e.EstablishmentUrn;
                        linkedEstabDataModel.Group = dataModel;
                        linkedEstabDataModel.JoinedDate = e.JoinedDate;
                        linkedEstabDataModel.CCIsLeadCentre = e.CCIsLeadCentre;
                        if (!e.Id.HasValue) _dbContext.EstablishmentGroups.Add(linkedEstabDataModel);
                    }

                    if (!dto.IsNewEntity)
                    {
                        var urns = dto.LinkedEstablishments.Select(x => x.EstablishmentUrn).ToArray();
                        var toRemove = _dbContext.EstablishmentGroups.Where(x => x.GroupUID == dto.Group.GroupUID && !urns.Contains(x.EstablishmentUrn)).ToList();
                        foreach (var item in toRemove)
                        {
                            item.IsDeleted = true;
                        }
                    }
                }
                
                foreach (var change in changes)
                {
                    _dbContext.GroupChangeHistories.Add(new GroupChangeHistory
                    {
                        ApproverUserId = _securityService.GetUserId(principal),
                        EffectiveDateUtc = DateTime.UtcNow,
                        Name = change.Name,
                        NewValue = change.NewValue,
                        OldValue = change.OldValue,
                        OriginatorUserId = _securityService.GetUserId(principal),
                        RequestedDateUtc = DateTime.UtcNow,
                        GroupUId = dataModel.GroupUID
                    });
                }
                
                await _dbContext.SaveChangesAsync();

                if (!newRecord) await _groupRepository.ClearRelationshipCacheAsync(dataModel.GroupUID);

                return dataModel.GroupUID;
            }
            
        }
    }
}
