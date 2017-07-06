using System.Collections.Generic;
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

        [HttpPost, Route("CreateChildrensCentre/Validate/JoinedDate")]
        public async Task<IHttpActionResult> ValidateEstablishmentJoinedDate(DateTimeViewModel model)
        {
            if (model == null || model.IsEmpty())
            {
                ModelState.AddModelError("model", "Date cannot be empty");
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
            return Json(validation.Errors.Select(e => new {e.Fields, e.Message}));
        }
    }
}
