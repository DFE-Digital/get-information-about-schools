using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.EditPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Web.UI.Models;
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
            var model = new EstablishmentModel { TypeId = (int) eLookupEstablishmentType.Academy1619Converter };
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

        #region RegistrationSuspendedId

        [Fact]
        public void ModelIEBT_MapsRegistrationSuspendedIdCorrectly()
        {
            var viewModel = new EditEstablishmentModel
            {
                RegistrationSuspended = RegistrationSuspendedStatus.EducationAndBoardingSuspended
            };

            var model = new EstablishmentModel { IEBTModel = new IEBTModel() };

            var mappedModel = new IEBTModel { RegistrationSuspendedId = ((int)viewModel.RegistrationSuspended).ToString() };

            var submittedKeys = new List<string> { nameof(mappedModel.RegistrationSuspendedId) };

            var props = typeof(IEBTModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (submittedKeys.Contains(prop.Name))
                {
                    var value = prop.GetValue(mappedModel);
                    prop.SetValue(model.IEBTModel, value);
                }
            }

            Assert.Equal("1", model.IEBTModel.RegistrationSuspendedId);
        }


        [Theory]
        [InlineData("1", "Education and boarding suspended")]
        [InlineData("2", "Education suspended")]
        [InlineData("3", "Not applicable")]
        [InlineData("4", "Not recorded")]
        public void EnumDisplayNameFor_MapsCorrectly(string input, string expected)
        {
            var enumVal = (RegistrationSuspendedStatus) int.Parse(input);
            var display = EnumExtensions.EnumDisplayNameFor(enumVal);
            Assert.Equal(expected, display);
        }

        [Fact]
        public void GetApprovalPolicies_IncludeRegistrationSuspendedIdWhenFalse()
        {
            var policy = new EstablishmentApprovalsPolicy
            {
                IEBTDetail = new IEBTFieldList<ApprovalPolicy>
                {
                    RegistrationSuspended =
                        new ApprovalPolicy { RequiresApproval = false, ApproverName = "IEBT" }
                }
            };

            var result = policy.GetFieldsRequiringApproval();

            Assert.DoesNotContain("IEBTModel.RegistrationSuspended", result);
        }

        [Fact]
        public void GetApprovalPolicies_IncludeRegistrationSuspendedIdWhenTrue()
        {
            var policy = new EstablishmentApprovalsPolicy
            {
                IEBTDetail = new IEBTFieldList<ApprovalPolicy>
                {
                    RegistrationSuspended =
                        new ApprovalPolicy { RequiresApproval = true, ApproverName = "IEBT" }
                }
            };

            var result = policy.GetFieldsRequiringApproval();

            Assert.Contains("IEBTModel.RegistrationSuspended", result);
        }
        #endregion

}
}
