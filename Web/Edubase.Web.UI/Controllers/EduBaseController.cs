using Autofac;
using Edubase.Services.Domain;
using Edubase.Web.UI.Helpers;
using FluentValidation.Mvc;
using MoreLinq;
using System.Linq;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    public class EduBaseController : Controller
    {
        protected new JsonCamelCaseResult Json(object data) => new JsonCamelCaseResult(data, JsonRequestBehavior.AllowGet);

        protected void AddApiErrorsToModelState(ApiError[] errors) => (errors ?? Enumerable.Empty<ApiError>()).ForEach(x => ModelState.AddModelError(string.Empty, x.GetMessage()));
    }
}