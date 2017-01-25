using Autofac;
using Edubase.Web.UI.Helpers;
using FluentValidation.Mvc;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    public class EduBaseController : Controller
    {
        protected new JsonCamelCaseResult Json(object data) => new JsonCamelCaseResult(data, JsonRequestBehavior.AllowGet);

    }
}