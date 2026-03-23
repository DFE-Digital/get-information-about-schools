using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Helpers
{
    internal sealed class FakeHttpContext : HttpContextBase
    {
        private readonly IPrincipal _user;
        private readonly HttpRequestBase _request = new FakeHttpRequest();

        public FakeHttpContext(IPrincipal user, NameValueCollection qs = null)
        {
            _user = user;
            _request = new FakeHttpRequest(qs);
        }

        public override IPrincipal User => _user;
        public override HttpRequestBase Request => _request;
    }

    internal sealed class FakeHttpRequest : HttpRequestBase
    {

        private readonly NameValueCollection _queryString;

        public FakeHttpRequest(NameValueCollection qs = null)
        {
            _queryString = qs ?? new NameValueCollection();
        }

        public override NameValueCollection QueryString => _queryString;

        public override string ApplicationPath => "/";
        public override string AppRelativeCurrentExecutionFilePath => "~/";
        public override string Path => "/";
        public override string RawUrl => "/";
    }
}
