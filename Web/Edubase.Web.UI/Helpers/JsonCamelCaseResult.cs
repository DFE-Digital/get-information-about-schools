using System;
using System.Text;
using System.Threading.Tasks;
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

        public Encoding ContentEncoding { get; set; } = Encoding.UTF8;
        public string ContentType { get; set; } = "application/json";
        public object Data { get; set; }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var response = context.HttpContext.Response;
            response.ContentType = ContentType;
            response.Headers["Content-Encoding"] = ContentEncoding.WebName;

            if (Data == null)
                return;

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var json = JsonConvert.SerializeObject(Data, jsonSerializerSettings);
            var buffer = ContentEncoding.GetBytes(json);
            await response.Body.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
