using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Web.UI.Areas.Establishments;
using Edubase.Web.UI.Areas.Establishments.Models;
using Xunit;
using static Edubase.Web.UI.Areas.Establishments.Models.CreateEstablishmentViewModel;

namespace Edubase.Web.UIUnitTests.Areas.Establishments
{
    public class CreateWizardNavigationSteps
    {
        public eEstabCreateSteps ActionStep { get; set; } = eEstabCreateSteps.PhaseOfEducation;
        public eEstabCreateSteps CurrentStep { get; set; } = eEstabCreateSteps.NameEntry;
        public eEstabCreateSteps PreviousStep { get; set; } = eEstabCreateSteps.Undefined;
    }

    public class CreateWizardNavigationServiceTests
    {
        [Theory]
        [MemberData(nameof(NextStepsData))]
        public void MoveToNextStep_ReturnsNextStepOrFalse(CreateWizardNavigationSteps init, int establishmentType, CreateWizardNavigationSteps expectedSteps, bool expectedResult)
        {
            var viewModel = new CreateEstablishmentViewModel { EstablishmentTypeId = establishmentType, ActionStep = init.ActionStep, CurrentStep = init.CurrentStep, PreviousStep = init.PreviousStep };
            var result = CreateWizardNavigationService.MoveToNextStep(viewModel);

            Assert.Equal(viewModel.ActionStep, expectedSteps.ActionStep);
            Assert.Equal(viewModel.CurrentStep, expectedSteps.CurrentStep);
            Assert.Equal(viewModel.PreviousStep, expectedSteps.PreviousStep);
            Assert.Equal(result, expectedResult);
        }

        [Theory]
        [MemberData(nameof(NavigateBackData))]
        public void MoveBack_ReturnsPreviousStep(CreateWizardNavigationSteps init, CreateWizardNavigationSteps expectedSteps, bool expectedResult)
        {
            var viewModel = new CreateEstablishmentViewModel { ActionStep = init.ActionStep, CurrentStep = init.CurrentStep, PreviousStep = init.PreviousStep };
            var result = CreateWizardNavigationService.NavigatedBack(viewModel);
            Assert.Equal(viewModel.ActionStep, expectedSteps.ActionStep);
            Assert.Equal(viewModel.CurrentStep, expectedSteps.CurrentStep);
            Assert.Equal(viewModel.PreviousStep, expectedSteps.PreviousStep);
            Assert.Equal(result, expectedResult);
        }

        private const int EstablishmentTypeAny = 1;
        private const int EstablishmentTypeChildrensCentre = 41;

        public static IEnumerable<object[]> NextStepsData() =>
            new List<object[]> {
                new object[] {
                    new CreateWizardNavigationSteps {
                        ActionStep = eEstabCreateSteps.PhaseOfEducation,
                        CurrentStep = eEstabCreateSteps.NameEntry,
                        PreviousStep = eEstabCreateSteps.Undefined},
                    EstablishmentTypeChildrensCentre,
                    new CreateWizardNavigationSteps {
                        ActionStep = eEstabCreateSteps.Completed,
                        CurrentStep = eEstabCreateSteps.CreateEntry,
                        PreviousStep = eEstabCreateSteps.NameEntry},
                    true},
                new object[] {
                    new CreateWizardNavigationSteps {
                        ActionStep = eEstabCreateSteps.EstabNumber,
                        CurrentStep = eEstabCreateSteps.PhaseOfEducation,
                        PreviousStep = eEstabCreateSteps.NameEntry},
                    EstablishmentTypeAny,
                    new CreateWizardNavigationSteps {
                        ActionStep = eEstabCreateSteps.Completed,
                        CurrentStep = eEstabCreateSteps.EstabNumber,
                        PreviousStep = eEstabCreateSteps.PhaseOfEducation},
                    true },
                new object[] {
                    new CreateWizardNavigationSteps {
                        ActionStep = eEstabCreateSteps.Completed,
                        CurrentStep = eEstabCreateSteps.EstabNumber,
                        PreviousStep= eEstabCreateSteps.PhaseOfEducation},
                    EstablishmentTypeAny,
                    new CreateWizardNavigationSteps {
                        ActionStep = eEstabCreateSteps.Completed,
                        CurrentStep = eEstabCreateSteps.EstabNumber,
                        PreviousStep= eEstabCreateSteps.PhaseOfEducation},
                    false},
                new object[] {
                    new CreateWizardNavigationSteps {
                        ActionStep = eEstabCreateSteps.Completed,
                        CurrentStep = eEstabCreateSteps.CreateEntry,
                        PreviousStep= eEstabCreateSteps.NameEntry},
                    EstablishmentTypeChildrensCentre,
                    new CreateWizardNavigationSteps {
                        ActionStep = eEstabCreateSteps.Completed,
                        CurrentStep = eEstabCreateSteps.CreateEntry,
                        PreviousStep= eEstabCreateSteps.NameEntry},
                    false}
            };

        public static IEnumerable<object[]> NavigateBackData() =>
            new List<object[]> {
                new object[] {
                    new CreateWizardNavigationSteps {
                        ActionStep = eEstabCreateSteps.NameEntry,
                        CurrentStep = eEstabCreateSteps.PhaseOfEducation,
                        PreviousStep = eEstabCreateSteps.NameEntry},
                    new CreateWizardNavigationSteps {
                        ActionStep = eEstabCreateSteps.PhaseOfEducation,
                        CurrentStep = eEstabCreateSteps.NameEntry,
                        PreviousStep = eEstabCreateSteps.Undefined},
                    true},
                new object[] {
                    new CreateWizardNavigationSteps {
                        ActionStep = eEstabCreateSteps.PhaseOfEducation,
                        CurrentStep = eEstabCreateSteps.EstabNumber,
                        PreviousStep = eEstabCreateSteps.PhaseOfEducation},
                    new CreateWizardNavigationSteps {
                        ActionStep = eEstabCreateSteps.EstabNumber,
                        CurrentStep = eEstabCreateSteps.PhaseOfEducation,
                        PreviousStep = eEstabCreateSteps.NameEntry},
                    true},
                new object[] {
                    new CreateWizardNavigationSteps {
                        ActionStep = eEstabCreateSteps.NameEntry,
                        CurrentStep = eEstabCreateSteps.CreateEntry,
                        PreviousStep= eEstabCreateSteps.NameEntry},
                    new CreateWizardNavigationSteps {
                        ActionStep = eEstabCreateSteps.PhaseOfEducation,
                        CurrentStep = eEstabCreateSteps.NameEntry,
                        PreviousStep= eEstabCreateSteps.Undefined},
                    true}
            };
    }
}
