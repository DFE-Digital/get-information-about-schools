using System.Linq;
using Edubase.Services.Domain;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;

namespace Edubase.Web.UI.Controllers
{
    public class EduBaseController : Controller
    {
        protected new JsonCamelCaseResult Json(object data) => new JsonCamelCaseResult(data);

        protected void AddApiErrorsToModelState(ApiError[] errors, string keyName = "") => (errors ?? Enumerable.Empty<ApiError>()).ForEach(x => ModelState.AddModelError(keyName, x.GetMessage()));
    }
}
