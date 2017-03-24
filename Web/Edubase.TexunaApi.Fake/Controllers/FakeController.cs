using System;
using System.Collections.Generic;
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


        [HttpPut, Route("configure/{method}")]
        public IHttpActionResult SetResponse(string uri, string method, [FromBody]object response)
        {
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
