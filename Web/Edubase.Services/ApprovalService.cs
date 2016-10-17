using Edubase.Data.Entity;
using Edubase.Services.Domain;
using System.Linq;
using System.Data;
using System.Data.Entity;
using Edubase.Common;
using System.Security.Claims;
using Edubase.Data.Identity;
using System;
using System.Security;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Edubase.Services
{
    public class ApprovalService
    {
        public async Task<string> AcceptAsync(ClaimsPrincipal currentUser, int approvalItemId)
        {
            using (var dc = new ApplicationDbContext())
            {
                var item = GetForApproveOrReject(dc, currentUser, approvalItemId);
                var establishment = dc.Establishments.Single(x => x.Urn == item.Urn);

                var prop = typeof(Establishment).GetProperty(item.Name);

                object newValue = null;
                if (item.NewValue.IsInteger()) newValue = item.NewValue.ToInteger();
                else newValue = item.NewValue;
                

                ReflectionHelper.SetProperty(establishment, item.Name, newValue);
                item.ProcessedDateUtc = DateTime.UtcNow;
                item.ProcessorUserId = currentUser.Identity.GetUserId();
                item.IsApproved = true;
                item.LastUpdatedUtc = DateTime.UtcNow;

                await dc.SaveChangesAsync();

                await new SmtpClient().SendMailAsync("kris.dyson@contentsupport.co.uk", ConfigurationManager.AppSettings["DataOwnerEmailAddress"], "Establishment change accepted",
                                $"For Establishment URN: {establishment.Urn}, the item '{item.Name}' has changed to '{item.NewValue}'");

                await new BusMessagingService().SendEstablishmentUpdateMessage(establishment);

                return item.Name;
            }
        }

        public string Reject(ClaimsPrincipal currentUser, int approvalItemId, string reason)
        {
            using (var dc = new ApplicationDbContext())
            {
                var item = GetForApproveOrReject(dc, currentUser, approvalItemId);
                item.ProcessedDateUtc = DateTime.UtcNow;
                item.ProcessorUserId = currentUser.Identity.GetUserId();
                item.IsRejected = true;
                item.RejectionReason = reason;
                item.LastUpdatedUtc = DateTime.UtcNow;
                dc.SaveChanges();

                new SmtpClient().Send("kris.dyson@contentsupport.co.uk", ConfigurationManager.AppSettings["DataOwnerEmailAddress"], "Establishment change rejected",
                                $"For Establishment URN: {item.Urn}, the your suggested change for item '{item.Name}' to '{item.NewValue}' has been rejected for the following reason: {item.RejectionReason}");

                return item.Name;
            }
        }

        private EstablishmentApprovalQueue GetForApproveOrReject(ApplicationDbContext dc, ClaimsPrincipal currentUser, int approvalItemId)
        {
            Validate(currentUser);
            var item = CreateQuery(dc, currentUser, approvalItemId: approvalItemId).FirstOrDefault();
            if (item == null)
            {
                item = dc.EstablishmentApprovalQueue.FirstOrDefault(x => x.Id == approvalItemId);
                if (item == null) throw new Exception("The item does not exist");
                if (item.IsApproved) throw new Exception("Item is already approved");
                if (item.IsRejected) throw new Exception("Item is already rejected");
                if (item.IsDeleted) throw new Exception("Item is deleted");
                string roleName = GetUserRestrictiveRole(currentUser);
                if (roleName == null) throw new SecurityException("Unable to ascertain permissions for user");
                if (!dc.Permissions.Any(x => x.AllowApproval == true && x.PropertyName == item.Name && x.RoleName == roleName))
                    throw new SecurityException("User does not have permission to accept/reject");
            }
            return item;
        }

        private string GetUserRestrictiveRole(ClaimsPrincipal currentUser)=> Roles.RestrictiveRoles.FirstOrDefault(x => currentUser.IsInRole(x));

        public ApprovalDto GetAll(ClaimsPrincipal currentUser, int skip = 0, int take = 10, int? establishmentUrn = null)
        {
            var retVal = new ApprovalDto(skip, take);
            Validate(currentUser);
            
            using (var dc = new ApplicationDbContext())
            {
                var query = CreateQuery(dc, currentUser, establishmentUrn);
                
                if (establishmentUrn.HasValue)
                {
                    retVal.EstablishmentUrn = establishmentUrn;
                    retVal.EstablishmentName = dc.Establishments.First(x => x.Urn == establishmentUrn).Name;
                }

                retVal.Count = query.Count();

                query = query.OrderBy(x => x.CreatedUtc).Skip(skip).Take(take);

                retVal.Items = query.Select(x => new ApprovalItemDto
                {
                    DateOfChange = x.CreatedUtc,
                    EstablishmentName = x.Establishment.Name,
                    EstablishmentUrn = x.Urn,
                    Id = x.Id,
                    OldValue = x.OldValue,
                    NewValue = x.NewValue,
                    UpdatedFieldName = x.Name,
                    OriginatorName = x.OriginatorUser.UserName
                }).ToList();

                var svc = new LookupService();
                foreach (var item in retVal.Items)
                {
                    if (svc.IsLookupField(item.UpdatedFieldName))
                    {
                        if (item.NewValue.IsInteger()) item.NewValue = svc.GetName(item.UpdatedFieldName, item.NewValue.ToInteger().Value);
                        if (item.OldValue.IsInteger()) item.OldValue = svc.GetName(item.UpdatedFieldName, item.OldValue.ToInteger().Value);
                    }
                }
            }
            return retVal;
        }

        public int Count(ClaimsPrincipal currentUser, int? establishmentUrn = null)
        {
            Validate(currentUser);
            using (var dc = new ApplicationDbContext())
            {
                var query = CreateQuery(dc, currentUser, establishmentUrn);
                return query.Count();   
            }
        }

        public bool Any(ClaimsPrincipal currentUser, int? establishmentUrn = null)
        {
            Validate(currentUser);
            using (var dc = new ApplicationDbContext())
            {
                var query = CreateQuery(dc, currentUser, establishmentUrn);
                return query.Any();
            }
        }

        private void Validate(ClaimsPrincipal currentUser)
        {
            if (currentUser == null) throw new ArgumentNullException(nameof(currentUser));
            if (!currentUser.Identity.IsAuthenticated)
                throw new SecurityException("Permission denied");
        }

        private IQueryable<EstablishmentApprovalQueue> CreateQuery(ApplicationDbContext dc, ClaimsPrincipal currentUser, int? establishmentUrn = null, int ? approvalItemId = null)
        {
            var q = dc.EstablishmentApprovalQueue
                .Include(x => x.Establishment)
                .Include(x => x.OriginatorUser)
                .AsQueryable();

            if (!currentUser.IsInRole(Roles.Admin))
            {
                var roleName = GetUserRestrictiveRole(currentUser);
                if (roleName == null) throw new SecurityException("The current user is not in a restrictive role or admin role; cannot determine permissions for this operation");
                q = q.Join(dc.Permissions, eaq => new { PropertyName = eaq.Name, RoleName = roleName, AllowApproval = true }, p => new { p.PropertyName, p.RoleName, p.AllowApproval }, (x, y) => x);
            }

            q = q.Where(x => x.IsDeleted == false 
                && (establishmentUrn == null || x.Urn == establishmentUrn) 
                && x.IsApproved == false 
                && x.IsRejected == false
                && (approvalItemId == null || x.Id == approvalItemId));

            return q;
        }

        

    }
}
