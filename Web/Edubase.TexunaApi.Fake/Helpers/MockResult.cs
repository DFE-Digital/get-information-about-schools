using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Edubase.TexunaApi.Fake.Helpers
{
    public class MockResult<T> : IHttpActionResult where T : IHttpActionResult
    {
        public T InnerResult { get; private set; }
        public string Id { get; private set; } = Guid.NewGuid().ToString("N");

        public MockResult(T innerResult)
        {
            InnerResult = innerResult;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = await InnerResult.ExecuteAsync(cancellationToken);
            response.Headers.Add("x-mock-response-id", Id);
            return response;
        }
    }
}