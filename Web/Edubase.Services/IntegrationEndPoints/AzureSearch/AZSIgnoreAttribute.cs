using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.AzureSearch
{
    /// <summary>
    /// Marks a property to be ignored when populating from an Azure Search
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class AZSIgnoreAttribute : Attribute
    {
        public AZSIgnoreAttribute()
        {

        }
    }
}
