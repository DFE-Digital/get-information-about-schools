using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Web.UI.Models;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Establishments.Models
{
    public class EstablishmentDetailViewModelTests
    {      
        [Theory]
        [InlineData(true, "http://ofsted.report.test/123456", true)]
        [InlineData(false, "http://ofsted.report.test/123456", false)]
        [InlineData(true, null, false)]
        [InlineData(true, "", false)]
        [InlineData(true, "   ", false)]
        public void OfstedReport_UsesApiValueAndDisplayPolicy(bool permitted, string url, bool expectedVisible)
        {
            var model = new EstablishmentDetailViewModel
            {
                Establishment = new EstablishmentModel
                {
                    OfstedReportUrl = url
                },
                DisplayPolicy = new EstablishmentDisplayEditPolicy
                {
                    OfstedReportUrl = permitted
                }
            };            

            Assert.Equal(url, model.OfstedReportUrl);
            Assert.Equal(expectedVisible, model.ShowOfstedReportLink);
        }
    }
}
