using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Edubase.Web.UI.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CannotBeEmptyAttribute : RequiredAttribute
    {
        public override bool IsValid(object value) => (value as IEnumerable)?.GetEnumerator().MoveNext() ?? false;
    }
}