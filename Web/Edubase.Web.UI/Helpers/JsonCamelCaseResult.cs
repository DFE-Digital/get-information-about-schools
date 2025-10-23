using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Edubase.Web.UI.Helpers
{
    public class JsonCamelCaseResult : IActionResult
    {
        public JsonCamelCaseResult(object data)
        {
            Data = data;
        }

        public Encoding ContentEncoding { get; set; }

        public string ContentType { get; set; } = "application/json";

        public object Data { get; set; }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var response = context.HttpContext.Response;
            response.ContentType = ContentType ?? "application/json";

            if (ContentEncoding != null)
            {
                response.Headers["Content-Encoding"] = ContentEncoding.WebName;
            }

            if (Data != null)
            {
                var json = JsonConvert.SerializeObject(Data, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                await response.WriteAsync(json, ContentEncoding ?? Encoding.UTF8);
            }
        }
    }
}
