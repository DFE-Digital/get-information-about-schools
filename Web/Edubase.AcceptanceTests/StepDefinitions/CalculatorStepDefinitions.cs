using Reqnroll.CommonModels;
using Xunit;

namespace Edubase.AcceptanceTests.StepDefinitions
{
    [Binding]
    public sealed class CalculatorStepDefinitions
    {
        private int firstNumber;
        private int secondNumber;
        private int actualResult;

        // For additional details on Reqnroll step definitions see https://go.reqnroll.net/doc-stepdef

        [Given("the first number is {int}")]
        public void GivenTheFirstNumberIs(int number)
        {
            firstNumber = number;
        }

        [Given("the second number is {int}")]
        public void GivenTheSecondNumberIs(int number)
        {
            secondNumber = number;
        }

        [When("the two numbers are added")]
        public void WhenTheTwoNumbersAreAdded()
        {
            actualResult = firstNumber + secondNumber;
        }

        [Then("the result should be {int}")]
        public void ThenTheResultShouldBe(int result)
        {
            Assert.Equal(result, actualResult);
        }
    }
}
