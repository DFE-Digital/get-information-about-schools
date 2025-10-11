using Edubase.Web.UI.Areas.Governors.Models;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Models
{
    public class GovernorViewModelTests
    {

        [Fact]
        public void EmptyViewModelHasEmptyFullName()
        {
            var governorViewModel = new GovernorViewModel();
            Assert.Equal(string.Empty, governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsTitle()
        {
            var arbitraryInt = 1;
            var arbitraryTitle = "Arbitrary Title";

            var governorViewModel = new GovernorViewModel()
            {
                GovernorTitleId = arbitraryInt,
                Titles = new []
                {
                    new SelectListItem() { Value = arbitraryInt.ToString(), Text = arbitraryTitle},
                }
            };
            Assert.Equal(arbitraryTitle, governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsTitleExceptWhenNotApplicable()
        {
            var arbitraryInt = 1;

            var governorViewModel = new GovernorViewModel()
            {
                GovernorTitleId = arbitraryInt,
                Titles = new []
                {
                    new SelectListItem() { Value = arbitraryInt.ToString(), Text = "Not-applicable"},
                }
            };
            Assert.Equal(string.Empty, governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsTitleExceptWhenNotRecorded()
        {
            var arbitraryInt = 1;

            var governorViewModel = new GovernorViewModel()
            {
                GovernorTitleId = arbitraryInt,
                Titles = new []
                {
                    new SelectListItem() { Value = arbitraryInt.ToString(), Text = "Not recorded"},
                }
            };
            Assert.Equal(string.Empty, governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsFirstName()
        {
            var arbitraryString = "ABC";
            var governorViewModel = new GovernorViewModel()
            {
                FirstName = arbitraryString,
            };
            Assert.Equal(arbitraryString, governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsMiddleName()
        {
            var arbitraryString = "ABC";
            var governorViewModel = new GovernorViewModel()
            {
                MiddleName = arbitraryString,
            };
            Assert.Equal(arbitraryString, governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsLastName()
        {
            var arbitraryString = "ABC";
            var governorViewModel = new GovernorViewModel()
            {
                LastName = arbitraryString,
            };
            Assert.Equal(arbitraryString, governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsFirstNameAndMiddleName()
        {
            var arbitraryFirstName = "Arbitrary FirstName";
            var arbitraryMiddleName = "Arbitrary MiddleName";
            var governorViewModel = new GovernorViewModel()
            {
                FirstName = arbitraryFirstName,
                LastName = arbitraryMiddleName,
            };
            Assert.Equal($"{arbitraryFirstName} {arbitraryMiddleName}", governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsFirstNameAndLastName()
        {
            var arbitraryFirstName = "Arbitrary FirstName";
            var arbitraryLastName = "Arbitrary LastName";
            var governorViewModel = new GovernorViewModel()
            {
                FirstName = arbitraryFirstName,
                LastName = arbitraryLastName,
            };
            Assert.Equal($"{arbitraryFirstName} {arbitraryLastName}", governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsFirstNameAndMiddleNameAndLastName()
        {
            var arbitraryFirstName = "Arbitrary FirstName";
            var arbitraryMiddleName = "Arbitrary MiddleName";
            var arbitraryLastName = "Arbitrary LastName";
            var governorViewModel = new GovernorViewModel()
            {
                FirstName = arbitraryFirstName,
                MiddleName = arbitraryMiddleName,
                LastName = arbitraryLastName,
            };
            Assert.Equal($"{arbitraryFirstName} {arbitraryMiddleName} {arbitraryLastName}", governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsTitleAndFirstName()
        {
            var arbitraryInt = 1;
            var arbitraryTitle = "Arbitrary Title";
            var arbitraryFirstName = "Arbitrary FirstName";

            var governorViewModel = new GovernorViewModel()
            {
                GovernorTitleId = arbitraryInt,
                Titles = new []
                {
                    new SelectListItem() { Value = arbitraryInt.ToString(), Text = arbitraryTitle},
                },
                FirstName = arbitraryFirstName,
            };
            Assert.Equal($"{arbitraryTitle} {arbitraryFirstName}", governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsTitleAndMiddleName()
        {
            var arbitraryInt = 1;
            var arbitraryTitle = "Arbitrary Title";
            var arbitraryMiddleName = "Arbitrary FirstName";

            var governorViewModel = new GovernorViewModel()
            {
                GovernorTitleId = arbitraryInt,
                Titles = new []
                {
                    new SelectListItem() { Value = arbitraryInt.ToString(), Text = arbitraryTitle},
                },
                FirstName = arbitraryMiddleName,
            };
            Assert.Equal($"{arbitraryTitle} {arbitraryMiddleName}", governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsTitleAndLastName()
        {
            var arbitraryInt = 1;
            var arbitraryTitle = "Arbitrary Title";
            var arbitraryLastName = "Arbitrary LastName";

            var governorViewModel = new GovernorViewModel()
            {
                GovernorTitleId = arbitraryInt,
                Titles = new []
                {
                    new SelectListItem() { Value = arbitraryInt.ToString(), Text = arbitraryTitle},
                },
                FirstName = arbitraryLastName,
            };
            Assert.Equal($"{arbitraryTitle} {arbitraryLastName}", governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsTitleAndFirstNameAndLastName()
        {
            var arbitraryInt = 1;
            var arbitraryTitle = "Arbitrary Title";
            var arbitraryFirstName = "Arbitrary FirstName";
            var arbitraryLastName = "Arbitrary LastName";

            var governorViewModel = new GovernorViewModel()
            {
                GovernorTitleId = arbitraryInt,
                Titles = new []
                {
                    new SelectListItem() { Value = arbitraryInt.ToString(), Text = arbitraryTitle},
                },
                FirstName = arbitraryFirstName,
                LastName = arbitraryLastName,
            };
            Assert.Equal($"{arbitraryTitle} {arbitraryFirstName} {arbitraryLastName}", governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsTitleAndFirstNameAndMiddleNameAndLastName()
        {
            var arbitraryInt = 1;
            var arbitraryTitle = "Arbitrary Title";
            var arbitraryFirstName = "Arbitrary FirstName";
            var arbitraryMiddleName = "Arbitrary MiddleName";
            var arbitraryLastName = "Arbitrary LastName";

            var governorViewModel = new GovernorViewModel()
            {
                GovernorTitleId = arbitraryInt,
                Titles = new []
                {
                    new SelectListItem() { Value = arbitraryInt.ToString(), Text = arbitraryTitle},
                },
                FirstName = arbitraryFirstName,
                MiddleName = arbitraryMiddleName,
                LastName = arbitraryLastName,
            };
            Assert.Equal($"{arbitraryTitle} {arbitraryFirstName} {arbitraryMiddleName} {arbitraryLastName}", governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsTitleAndFirstNameAndMiddleNameAndLastNameAndValuesAreTrimmed()
        {
            var arbitraryInt = 1;
            var arbitraryTitle = "  Arbitrary Title  ";
            var arbitraryFirstName = "  Arbitrary FirstName  ";
            var arbitraryMiddleName = "  Arbitrary MiddleName  ";
            var arbitraryLastName = "  Arbitrary LastName  ";

            var governorViewModel = new GovernorViewModel()
            {
                GovernorTitleId = arbitraryInt,
                Titles = new []
                {
                    new SelectListItem() { Value = arbitraryInt.ToString(), Text = arbitraryTitle},
                },
                FirstName = arbitraryFirstName,
                MiddleName = arbitraryMiddleName,
                LastName = arbitraryLastName,
            };
            Assert.Equal($"{arbitraryTitle.Trim()} {arbitraryFirstName.Trim()} {arbitraryMiddleName.Trim()} {arbitraryLastName.Trim()}", governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsTitleAndFirstNameAndMiddleNameAndLastNameAndValuesAreTrimmedAtEndsOnly()
        {
            var arbitraryInt = 1;
            var arbitraryTitle = "  Arbitrary   Title  ";
            var arbitraryFirstName = "  Arbitrary   FirstName  ";
            var arbitraryMiddleName = "  Arbitrary   MiddleName  ";
            var arbitraryLastName = "  Arbitrary   LastName  ";

            var governorViewModel = new GovernorViewModel()
            {
                GovernorTitleId = arbitraryInt,
                Titles = new []
                {
                    new SelectListItem() { Value = arbitraryInt.ToString(), Text = arbitraryTitle},
                },
                FirstName = arbitraryFirstName,
                MiddleName = arbitraryMiddleName,
                LastName = arbitraryLastName,
            };
            Assert.Equal($"{arbitraryTitle.Trim()} {arbitraryFirstName.Trim()} {arbitraryMiddleName.Trim()} {arbitraryLastName.Trim()}", governorViewModel.FullName);
        }
    }
}
