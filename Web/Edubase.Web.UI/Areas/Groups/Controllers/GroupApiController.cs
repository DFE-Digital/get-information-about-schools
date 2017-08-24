using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Areas.Groups.Controllers
{
    [RoutePrefix("Groups/Group")]
    public class GroupApiController : ApiController
    {
        private readonly IGroupsWriteService groupsWriteService;

        public GroupApiController(IGroupsWriteService groupsWriteService)
        {
            this.groupsWriteService = groupsWriteService;
        }

        [HttpPost, Route("CreateChildrensCentre/Validate")]
        public async Task<IHttpActionResult> ValidateChildrensCentreGroup(ValidateChildrensCentreStep2 model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.Where(m => m.Value.Errors.Any()));
            }

            return Json(new string[] { });
        }

        [HttpPost, Route("CreateChildrensCentre/Validate/OpenDate")]
        public async Task<IHttpActionResult> ValidateGroupOpenDate(DateTimeViewModel openDate)
        {
            if (openDate == null || openDate.IsEmpty())
            {
                ModelState.AddModelError("openDate", "Date cannot be empty");
            }

            if (!ModelState.IsValid)
            {
                return Json(ModelState.Where(m => m.Value.Errors.Any()));
            }

            return Json(new string[] { });
        }

        [HttpPost, Route("CreateChildrensCentre/Validate/JoinedDate")]
        public async Task<IHttpActionResult> ValidateEstablishmentJoinedDate(DateTimeViewModel joinDate, DateTimeViewModel groupOpenDate, string groupType)
        {
            if (joinDate == null || joinDate.IsEmpty())
            {
                ModelState.AddModelError("joinDate", "Join date cannot be empty");
            }

            if (groupOpenDate == null || groupOpenDate.IsEmpty())
            {
                ModelState.AddModelError("groupOpenDate", "Group open date cannot be empty");
            }

            if (joinDate.ToDateTime().Value.Date < groupOpenDate.ToDateTime().Value.Date)
            {
                var part = (groupOpenDate.ToDateTime().Value.Date == DateTime.Now.Date) ? $"the {groupType}'s creation date of today" : $"the {groupType}'s creation date of {groupOpenDate.Day}/{groupOpenDate.Month}/{groupOpenDate.Year}";
                var message = $"The join date you enetered is before {part}. Please enter a later date.";
                ModelState.AddModelError("joinDate", message);
            }

            if (!ModelState.IsValid)
            {
                return Json(ModelState.Where(m => m.Value.Errors.Any()));
            }

            return Json(new string[] { });
        }

        [HttpPost, Route("CreateChildrensCentre/Validate/All")]
        public async Task<IHttpActionResult> ValidateGroupWithEstablishments(ValidateCCGroupWithEstablishments model)
        {
            var dto = new SaveGroupDto
            {
                Group = new GroupModel
                {
                    GroupTypeId = model?.GroupTypeId,
                    LocalAuthorityId = model?.LocalAuthorityId,
                    Name = model?.Name,
                    OpenDate = model?.OpenDate,
                },
                LinkedEstablishments = model?.Establishments?.Select(e => new LinkedEstablishmentGroup
                    {
                        CCIsLeadCentre = e?.CCIsLeadCentre ?? false,
                        Urn = e?.Urn,
                        JoinedDate = e?.JoinedDate
                    })
                    .ToList()
            };

            var validation = await groupsWriteService.ValidateAsync(dto, User);
            return Json(validation);
        }
    }
}
