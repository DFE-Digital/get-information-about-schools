using Edubase.Web.UI.Areas.Establishments.Models;
using CreateSteps = Edubase.Web.UI.Areas.Establishments.Models.CreateEstablishmentViewModel.eEstabCreateSteps;

namespace Edubase.Web.UI.Areas.Establishments
{
    public static class CreateWizardNavigationService
    {
        public static bool NavigatedBack(CreateChildrensCentreViewModel viewModel)
        {
            if (viewModel.ActionStep < viewModel.CurrentStep)
            {
                viewModel.ActionStep = viewModel.CurrentStep == CreateSteps.CreateEntry ? CreateSteps.PhaseOfEducation : viewModel.CurrentStep;
                viewModel.CurrentStep = viewModel.PreviousStep;
                viewModel.PreviousStep = viewModel.PreviousStep - 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool RouteIsComplete(CreateChildrensCentreViewModel viewModel) => viewModel.ActionStep == CreateSteps.Completed;

        public static bool MoveToNextStep(CreateChildrensCentreViewModel viewModel)
        {
            if (viewModel.ActionStep != CreateSteps.Completed)
            {
                viewModel.PreviousStep = viewModel.CurrentStep;
                viewModel.CurrentStep = viewModel.ActionStep;
            }
            if (viewModel.ActionStep == CreateSteps.PhaseOfEducation)
            {
                if (viewModel.EstablishmentTypeId == 41)
                {
                    viewModel.CurrentStep = CreateSteps.CreateEntry;
                    viewModel.ActionStep = CreateSteps.Completed;
                    return true;
                }
                else
                {
                    viewModel.ActionStep = CreateSteps.EstabNumber;
                    return true;
                }
            }
            else if (viewModel.ActionStep == CreateSteps.EstabNumber)
            {
                viewModel.CurrentStep = CreateSteps.EstabNumber;
                viewModel.ActionStep = CreateSteps.Completed;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
