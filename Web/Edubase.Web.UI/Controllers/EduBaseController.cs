using System.Linq;
using Edubase.Services.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    public class EduBaseController : Controller
    {
        protected IActionResult JsonCamel(object data) => Json(data);

        protected void AddApiErrorsToModelState(ApiError[] errors, string keyName = "")
        {
            foreach (var error in errors ?? Enumerable.Empty<ApiError>())
            {
                ModelState.AddModelError(keyName, error.GetMessage());
            }
        }
    }
}
