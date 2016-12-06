using Edubase.Services.Establishments.Search;
using NUnit.Framework;

namespace Edubase.UnitTest.Services.Establishments.Search
{
    [TestFixture]
    public class EstablishmentSearchFiltersTest
    {
        [Test]
        public void ValidateODataFilterExpression_EducationPhaseId()
        {
            var subject = new EstablishmentSearchFilters();
            subject.EducationPhaseIds = new int[] { 1, 2, 3 };
            Assert.AreEqual("(EducationPhaseId eq 1 or EducationPhaseId eq 2 or EducationPhaseId eq 3)", subject.ToString());
        }

        [Test]
        public void ValidateODataFilterExpression_EducationPhaseId_TypeId_EstablishmentTypeGroupId()
        {
            var subject = new EstablishmentSearchFilters();
            subject.EducationPhaseIds = new int[] { 1 };
            subject.TypeIds = new int[] { 111 };
            subject.EstablishmentTypeGroupIds = new int[] { 1, 5 };
            Assert.AreEqual("(EducationPhaseId eq 1) and (TypeId eq 111) and (EstablishmentTypeGroupId eq 1 or EstablishmentTypeGroupId eq 5)", subject.ToString());
        }

        [Test]
        public void ValidateODataFilterExpression_EducationPhaseId_TypeId_EstablishmentTypeGroupId_LAESTAB()
        {
            var subject = new EstablishmentSearchFilters();
            subject.EducationPhaseIds = new int[] { 1 };
            subject.TypeIds = new int[] { 111 };
            subject.EstablishmentTypeGroupIds = new int[] { 1, 5 };
            subject.EstablishmentNumber = 3424;
            subject.LocalAuthorityIds = new int[] { 201 };
            Assert.AreEqual("(LocalAuthorityId eq 201) and (EducationPhaseId eq 1) and (TypeId eq 111) and (EstablishmentTypeGroupId eq 1 or EstablishmentTypeGroupId eq 5) and (EstablishmentNumber eq 3424)", subject.ToString());
        }

        [Test]
        public void ValidateODataFilterExpression_Empty()
        {
            var subject = new EstablishmentSearchFilters();
            Assert.AreEqual("", subject.ToString());
        }
    }
}
