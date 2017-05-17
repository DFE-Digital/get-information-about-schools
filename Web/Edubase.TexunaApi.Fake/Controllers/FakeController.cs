using Edubase.TexunaApi.Fake.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Edubase.TexunaApi.Fake.Controllers
{
    public class FakeController : ApiController
    {
        private static readonly Lazy<Dictionary<string, MockMessage>> LazyDictionary = new Lazy<Dictionary<string, MockMessage>>();
        private static readonly Lazy<Dictionary<Guid, MockMessage>> LazyIncomingRequestPayloads = new Lazy<Dictionary<Guid, MockMessage>>();

        private static Dictionary<string, MockMessage> ConfiguredResponses => LazyDictionary.Value;
        private static Dictionary<Guid, MockMessage> IncomingRequestPayloads => LazyIncomingRequestPayloads.Value;


        [HttpGet]
        public IHttpActionResult Get(string uri)
        {
            var key = $"get-{uri}";
            if (ConfiguredResponses.ContainsKey(key))
            {
                return ResponseMessage(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = ConfiguredResponses[key].ToHttpContent()
                });
            }

            return this.NotFound();
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post(string uri)
        {
            var key = $"post-{uri}";
            if (ConfiguredResponses.ContainsKey(key))
            {
                var content = ConfiguredResponses[key].ToHttpContent();
                IncomingRequestPayloads.Add(content.Id, new MockMessage
                {
                    Content = await Request.Content.ReadAsStringAsync(),
                    ContentType = Request.Content.Headers.ContentType
                });

                return ResponseMessage(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                });
            }

            return this.NotFound();
        }

        [HttpGet, Route("configure")]
        public IHttpActionResult GetConfiguredResponses()
        {
            return this.Ok(ConfiguredResponses.Select(response =>
            {
                var hyphen = response.Key.IndexOf("-");
                var method = response.Key.Substring(0, hyphen);
                var url = response.Key.Substring(hyphen + 1);
                return new {Method = method, Url = url, Response = response.Value};
            }));
        }

        [HttpGet, Route("throwexception")]
        public IHttpActionResult ThrowException()
        {
            throw new Exception("Test exception");
        }

        [HttpGet, Route("_request-payload/{id}")]
        public IHttpActionResult GetRequestPayload(Guid id)
        {
            if (IncomingRequestPayloads.ContainsKey(id)) return Ok(IncomingRequestPayloads[id]);
            else return NotFound();
        }


        [HttpPut, Route("configure/{method}")]
        public async Task<IHttpActionResult> SetResponse(string uri, string method)
        {
            if (uri.StartsWith("/")) throw new Exception("The URI cannot start with a slash");

            var response = new MockMessage
            {
                Content = await Request.Content.ReadAsStringAsync(),
                ContentType = Request.Content.Headers.ContentType
            };

            var paramsStart = uri.IndexOf("?");
            if (paramsStart > -1)
            {
                uri = uri.Substring(0, paramsStart);
            }

            var key = $"{method}-{uri}";
            if (ConfiguredResponses.ContainsKey(key))
            {
                return this.Conflict();
            }

            ConfiguredResponses.Add(key, response);
            return this.Ok($"Response configured as {response.ContentType}");
        }

        [HttpDelete, Route("configure/{method}")]
        public IHttpActionResult DeleteResponse(string uri, string method)
        {
            var key = $"{method}-{uri}";
            if (ConfiguredResponses.ContainsKey(key))
            {
                ConfiguredResponses.Remove(key);
            }

            return this.Ok();
        }

        [HttpDelete, Route("configure")]
        public IHttpActionResult DeleteAllResponses()
        {
            ConfiguredResponses.Clear();

            return this.Ok();
        }
    }
}
