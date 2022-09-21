using Xunit;
using Edubase.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Domain;

namespace Edubase.Web.UI.Models.Tests
{
    public class EditEstablishmentModelTests
    {
        [Fact]
        public void UpdatingLocalAuthorityIdUpdatesLocalAuthorityCode()
        {
            var model = new EditEstablishmentModel();

            var la1 = new LookupDto() { Id = 1, Code = "AAA", Name = "TestLa1", DisplayOrder = 1 };
            var la2 = new LookupDto() { Id = 2, Code = "BBB", Name = "TestLa2", DisplayOrder = 2 };

            var localAuthorities = new List<LookupDto>() { la1, la2 };

            model.LocalAuthorities = localAuthorities.ToSelectList();
            Assert.Null(model.LocalAuthorityCode);
            model.LocalAuthorityId = la1.Id;
            Assert.Equal(la1.Code, model.LocalAuthorityCode);
            model.LocalAuthorityId = la2.Id;
            Assert.Equal(la2.Code, model.LocalAuthorityCode);
        }
    }
}
