using System.ComponentModel.DataAnnotations;

namespace Edubase.Web.UI.Helpers
{
    public class RequireUrlNotification : ValidationAttribute
    {
        private readonly string _dependent;

        public RequireUrlNotification(string dependent)
        {
            _dependent = dependent;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dependentProperty = validationContext.ObjectType.GetProperty(_dependent);
            if (dependentProperty == null)
            {
                return new ValidationResult("The dependent property could not be found.");
            }

            var dependentValue = dependentProperty.GetValue(validationContext.ObjectInstance, null) as string;
            var thisValue = value as string;

            if (!string.IsNullOrWhiteSpace(dependentValue) && string.IsNullOrWhiteSpace(thisValue))
            {
                return new ValidationResult(ErrorMessage ?? "The URL link must be entered when the message is populated.");
            }

            return ValidationResult.Success;
        }
    }
}
