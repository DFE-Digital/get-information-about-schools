using Edubase.Common;
using Edubase.Data.DbContext;
using Edubase.Data.Entity;
using Edubase.Services.Domain;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Services
{
    [Obsolete("Becoming obsolete!  use EstablishmentReadService")]
    public class EstablishmentService
    {
        public string GetName(int urn)
        {
            using (var dc = new ApplicationDbContext())
                return dc.Establishments.FirstOrDefault(x => x.Urn == urn)?.Name;
        }
        
        public void AddChangeHistory(int urn, ApplicationDbContext dc, string approverUserId, string originatorUserId, DateTime requestedDate, params ChangeDescriptor[] changes)
        {
            changes.ForEach(x => dc.EstablishmentChangeHistories.Add(new EstablishmentChangeHistory
            {
                ApproverUserId = approverUserId,
                EffectiveDateUtc = DateTime.UtcNow,
                Name = x.Name,
                OldValue = x.OldValue,
                NewValue = x.NewValue,
                RequestedDateUtc = requestedDate,
                OriginatorUserId = originatorUserId,
                Urn = urn
            }));
        }

        //public async Task SaveChangeHistoryAsync(int urn, string approverUserId, string originatorUserId, DateTime requestedDate, params ChangeDescriptor[] changes)
        //{
        //    using (var dc = new ApplicationDbContext())
        //    {
        //        AddChangeHistory(urn, dc, approverUserId, originatorUserId, requestedDate, changes);
        //        await dc.SaveChangesAsync();
        //    }
        //}
        
        //public async Task<EstablishmentChangeDto[]> GetChangeHistoryAsync(int urn, ApplicationDbContext dc = null)
        //{
        //    var changes = await ApplicationDbContext.OperationAsync(async dataContext =>
        //    {
        //        return await dataContext.EstablishmentChangeHistories
        //            .Include(x => x.OriginatorUser)
        //            .Include(x => x.ApproverUser)
        //            .Where(x => x.Urn == urn)
        //            .OrderByDescending(x => x.EffectiveDateUtc)
        //            .Select(x => new EstablishmentChangeDto
        //            {
        //                PropertyName = x.Name,
        //                ApproverUserId = x.ApproverUserId,
        //                EffectiveDateUtc = x.EffectiveDateUtc,
        //                Id = x.Id,
        //                NewValue = x.NewValue,
        //                OldValue = x.OldValue,
        //                OriginatorUserId = x.OriginatorUserId,
        //                RequestedDateUtc = x.RequestedDateUtc,
        //                Urn = x.Urn,
        //                ApproverUserName = x.ApproverUser.UserName,
        //                OriginatorUserName = x.OriginatorUser.UserName
        //            }).ToArrayAsync();
        //    });

        //    var cachedLookupService = new CachedLookupService();
        //    changes.ForEach(async x =>
        //    {
        //        if (cachedLookupService.IsLookupField(x.PropertyName))
        //        {
        //            if (x.OldValue.IsInteger())
        //                x.OldValue = await cachedLookupService.GetNameAsync(x.PropertyName, x.OldValue.ToInteger().Value);
                    
        //            if (x.NewValue.IsInteger())
        //                x.NewValue = await cachedLookupService.GetNameAsync(x.PropertyName, x.NewValue.ToInteger().Value);
        //        }
        //    });
        //    return changes;
        //}
        


    }
}
