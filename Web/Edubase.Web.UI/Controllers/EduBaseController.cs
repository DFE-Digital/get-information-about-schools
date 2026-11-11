using System;
using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Web.Resources;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    public class EduBaseController : Controller
    {
        protected new JsonCamelCaseResult Json(object data) => new JsonCamelCaseResult(data, JsonRequestBehavior.AllowGet);

        protected JsonCamelCaseResult Json(object data, JsonRequestBehavior jsonRequestBehavior) => new JsonCamelCaseResult(data, jsonRequestBehavior);

        protected void AddApiErrorsToModelState(ApiError[] errors, string keyName = "")
        {
            foreach (var error in errors ?? Array.Empty<ApiError>())
            {
                var message = ApiMessagesHelper.Get(error.Code).Clean() ??
                    ApiMessagesHelper.Get(string.Concat(error.Code, "_", error.Fields), error.Message);

                ModelState.AddModelError(keyName, message);
            }
        }
    }
}
