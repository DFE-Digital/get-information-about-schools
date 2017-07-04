using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Areas.Groups.Controllers
{
    [RoutePrefix("Groups/Group")]
    public class GroupApiController : ApiController
    {
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
    }
}
