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
            return await Task.Run(() =>
                !ModelState.IsValid ? Json(ModelState.Where(m => m.Value.Errors.Any()))
                    : (IHttpActionResult) Json(new string[] { }));
        }

        [HttpPost, Route("CreateChildrensCentre/Validate/OpenDate")]
        public async Task<IHttpActionResult> ValidateGroupOpenDate(DateTimeViewModel openDate)
        {
            if (openDate == null || openDate.IsEmpty())
            {
                ModelState.AddModelError("openDate", "Date cannot be empty");
            } else if (openDate.IsValid() == false)
            {
                ModelState.AddModelError("openDate", "The date specified is not valid");
            }
            return await Task.Run(() =>
                !ModelState.IsValid ? Json(ModelState.Where(m => m.Value.Errors.Any()))
                    : (IHttpActionResult) Json(new string[] { }));
        }

        [HttpPost, Route("CreateChildrensCentre/Validate/JoinedDate")]
        public async Task<IHttpActionResult> ValidateEstablishmentJoinedDate(ValidateEstablishmentJoinedDateModel model)
        {
            if (model.JoinDate == null || model.JoinDate.IsEmpty())
            {
                ModelState.AddModelError("joinDate", "Join date cannot be empty");
            }
            else if (model.JoinDate.IsValid() == false)
            {
                ModelState.AddModelError("joinDate", "The date specified is not valid");
            }

            if (model.GroupOpenDate == null || model.GroupOpenDate.IsEmpty())
            {
                ModelState.AddModelError("groupOpenDate", "Group open date cannot be empty");
            }
            else if (model.GroupOpenDate.IsValid() == false)
            {
                ModelState.AddModelError("groupOpenDate", "The date specified is not valid");
            }

            if (model.JoinDate.IsValid() && model.GroupOpenDate.IsValid() &&
                model.JoinDate.ToDateTime().Value.Date < model.GroupOpenDate.ToDateTime().Value.Date)
            {
                var part = (model.GroupOpenDate.ToDateTime().Value.Date == DateTime.Now.Date)
                    ? $"the {model.GroupType}'s open date of today"
                    : $"the {model.GroupType}'s open date of {model.GroupOpenDate.Day}/{model.GroupOpenDate.Month}/{model.GroupOpenDate.Year}";
                var message = $"The join date you entered is before {part}. Please enter a later date.";
                ModelState.AddModelError("joinDate", message);
            }

            return await Task.Run(() =>
                !ModelState.IsValid ? Json(ModelState.Where(m => m.Value.Errors.Any()))
                    : (IHttpActionResult) Json(new string[] { }));
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
            validation.Errors.ForEach(x => x.Message = x.GetMessage());
            return Json(validation);
        }
    }
}
