using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Edubase.TexunaApi.Fake.Controllers
{
    public class FakeController : ApiController
    {
        private static readonly Lazy<Dictionary<string, object>> LazyDictionary = new Lazy<Dictionary<string, object>>();
        private static Dictionary<string, object> ConfiguredResponses => LazyDictionary.Value;

        [HttpGet]
        public IHttpActionResult Get(string uri)
        {
            var key = $"get-{uri}";
            if (ConfiguredResponses.ContainsKey(key))
            {
                return this.Ok(ConfiguredResponses[key]);
            }

            return this.NotFound();
        }

        [HttpPost]
        public IHttpActionResult Post(string uri, object body)
        {
            var key = $"post-{uri}";
            if (ConfiguredResponses.ContainsKey(key))
            {
                return this.Ok(ConfiguredResponses[key]);
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


        [HttpPut, Route("configure/{method}")]
        public IHttpActionResult SetResponse(string uri, string method, [FromBody]object response)
        {
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
            return this.Ok();
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
