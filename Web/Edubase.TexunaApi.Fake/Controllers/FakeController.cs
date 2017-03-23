using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Edubase.TexunaApi.Fake.Controllers
{
    public class FakeController : ApiController
    {
        private static readonly Lazy<Dictionary<string, object>> lazyDictionary = new Lazy<Dictionary<string, object>>();
        private static Dictionary<string, object> ConfiguredResponses => lazyDictionary.Value;

        [HttpGet]
        public IHttpActionResult Get(string uri)
        {
            if (ConfiguredResponses.ContainsKey(uri))
            {
                return this.Ok(ConfiguredResponses[uri]);
            }

            return this.NotFound();
        }

        [HttpPut, Route("Configure")]
        public IHttpActionResult SetResponse(string uri, [FromBody]object response)
        {
            if (ConfiguredResponses.ContainsKey(uri))
            {
                return this.Conflict();
            }

            ConfiguredResponses.Add(uri, response);
            return this.Ok();
        }

        [HttpDelete, Route("Configure")]
        public IHttpActionResult DeleteResponse(string uri)
        {
            if (ConfiguredResponses.ContainsKey(uri))
            {
                ConfiguredResponses.Remove(uri);
            }

            return this.Ok();
        }
    }
}
