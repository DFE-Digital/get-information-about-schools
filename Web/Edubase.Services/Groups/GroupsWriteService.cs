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


        public GroupsWriteService(IApplicationDbContext dbContext, IEstablishmentReadService establishmentsReadService, 
            ISecurityService securityService, IGroupReadService groupReadService)
        {
            _dbContext = dbContext;
            _establishmentsReadService = establishmentsReadService;
            _securityService = securityService;
            _groupReadService = groupReadService;
        }

        public async Task<int> SaveAsync(SaveGroupDto dto, IPrincipal principal)
        {
            var validator = new SaveGroupDtoValidator(principal, _establishmentsReadService, _securityService, _groupReadService).Validate(dto);
            if (!validator.IsValid) throw new AggregateException(validator.Errors.Select(x => new EdubaseException(x.ErrorMessage)));
            else
            {
                if(dto.Group.GroupTypeId == (int)GT.ChildrensCentresGroup)
                {
                    var leadCentreUrn = dto.LinkedEstablishments.Single(x => x.CCIsLeadCentre).EstablishmentUrn;
                    var e = (await _establishmentsReadService.GetAsync(leadCentreUrn, principal)).GetResult();
                    dto.Group.Address = e.GetAddress();
                }
                
                var dataModel = !dto.IsNewEntity ? _dbContext.Groups.SingleOrThrow(x => x.GroupUID == dto.Group.GroupUID) : new GroupCollection();
                dataModel.Address = dto.Group.Address;
                dataModel.ClosedDate = dto.Group.ClosedDate;
                dataModel.CompaniesHouseNumber = dto.Group.CompaniesHouseNumber;
                dataModel.EstablishmentCount = dto.LinkedEstablishments.Count;
                dataModel.GroupId = dto.Group.GroupId;
                dataModel.GroupTypeId = dto.Group.GroupTypeId;
                dataModel.LocalAuthorityId = dto.Group.LocalAuthorityId;
                dataModel.ManagerEmailAddress = dto.Group.ManagerEmailAddress;
                dataModel.Name = dto.Group.Name;
                dataModel.OpenDate = dto.Group.OpenDate;
                dataModel.StatusId = dto.Group.StatusId;
                if (dto.IsNewEntity) _dbContext.Groups.Add(dataModel);

                foreach (var e in dto.LinkedEstablishments)
                {
                    var linkedEstabDataModel = e.Id.HasValue ? _dbContext.EstablishmentGroups.SingleOrThrow(x => x.Id == e.Id) : new EstablishmentGroup();
                    linkedEstabDataModel.EstablishmentUrn = e.EstablishmentUrn;
                    linkedEstabDataModel.Group = dataModel;
                    linkedEstabDataModel.JoinedDate = e.JoinedDate;
                    //linkedEstabDataModel.CCIsLeadCentre = e.CCIsLeadCentre; todo!
                    if (!e.Id.HasValue) _dbContext.EstablishmentGroups.Add(linkedEstabDataModel);
                }
                
                // todo: clear cache on this item, if existing record.

                await _dbContext.SaveChangesAsync();
                return dataModel.GroupUID;
            }
        }
    }
}
