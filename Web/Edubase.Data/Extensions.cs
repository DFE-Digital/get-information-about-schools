using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using MoreLinq;

namespace Edubase.Data
{
    internal static class Extensions
    {
        public static string ExtractValidationErrorsReport(this DbEntityValidationException exception)
        {
            var sb = new StringBuilder();
            exception.EntityValidationErrors.SelectMany(x => x.ValidationErrors).ForEach(x => sb.AppendLine(x.PropertyName + ", " + x.ErrorMessage));
            return sb.ToString();
        }
    }
}
