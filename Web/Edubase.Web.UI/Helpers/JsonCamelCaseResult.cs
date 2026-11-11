using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Edubase.Web.UI.Helpers
{
    public enum JsonRequestBehavior
    {
        AllowGet,
        DenyGet
    }

    public class JsonCamelCaseResult : ActionResult
    {
        public JsonCamelCaseResult(object data, JsonRequestBehavior jsonRequestBehavior)
        {
            Data = data;
            JsonRequestBehavior = jsonRequestBehavior;
        }

        public Encoding ContentEncoding { get; set; }

        public string ContentType { get; set; }

        public object Data { get; set; }

        public JsonRequestBehavior JsonRequestBehavior { get; set; }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var method = context.HttpContext.Request.Method;
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.");
            }

            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
            {
                response.Headers.ContentEncoding = ContentEncoding.WebName;
            }

            if (Data == null)
            {
                return;
            }

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            await response.WriteAsync(JsonConvert.SerializeObject(Data, jsonSerializerSettings), ContentEncoding ?? Encoding.UTF8);
        }
    }
}
