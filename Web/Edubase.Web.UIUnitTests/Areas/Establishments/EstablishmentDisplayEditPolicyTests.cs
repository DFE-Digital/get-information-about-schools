using Edubase.Services.Enums;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Establishments
{
    public class EstablishmentDisplayEditPolicyTests
    {
        [Theory]
        [InlineData((int) eLookupEstablishmentType.OtherIndependentSchool)]
        [InlineData((int) eLookupEstablishmentType.OtherIndependentSpecialSchool)]
        public void Initialise_Sets_RegistrationSuspended_True_For_ExpectedTypes(int typeId)
        {
            var model = new EstablishmentModel { TypeId = typeId };
            var policy = new EstablishmentDisplayEditPolicy();

            policy.Initialise(model);

            Assert.True(policy.IEBTDetail.RegistrationSuspended);
        }

        [Fact]
        public void Initialise_Sets_RegistrationSuspended_False_For_OtherTypes()
        {
            var model = new EstablishmentModel { TypeId = (int)eLookupEstablishmentType.Academy1619Converter };
            var policy = new EstablishmentDisplayEditPolicy();

            policy.Initialise(model);

            Assert.False(policy.IEBTDetail.RegistrationSuspended);
        }

        [Fact]
        public void Initialise_Sets_RegistrationSuspended_False_For_OtherType2s()
        {
            var model = new EstablishmentModel { TypeId = null };
            var policy = new EstablishmentDisplayEditPolicy();

            policy.Initialise(model);

            Assert.False(policy.IEBTDetail.RegistrationSuspended);
        }

    }
}
