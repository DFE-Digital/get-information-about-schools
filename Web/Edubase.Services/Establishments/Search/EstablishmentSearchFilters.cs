using Edubase.Common;
using Edubase.Common.Reflection;
using Edubase.Common.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using Newtonsoft.Json;

namespace Edubase.Services.Establishments.Search
{
    public class EstablishmentSearchFilters : EstablishmentSearchFiltersLookups
    {
        [JsonIgnore]
        public int? UKPRN { get; set; }

        [JsonIgnore]
        public int? EstablishmentNumber { get; set; }

        public List<string> ToODataPredicateList(params string[] additionalPredicates)
        {
            var props = ReflectionHelper.GetProperties(this);
            var predicates = props.Select(x => OrExpression(ReflectionHelper.GetPropertyInfo(this, x))).Where(x => !x.IsNullOrEmpty()).ToList();
            if(additionalPredicates!= null && additionalPredicates.Any())
            {
                additionalPredicates.ForEach(x => predicates.Add(x));
            }
            return predicates;
        }

        /// <summary>
        /// Returns an OData filter expression
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {   
            return string.Join(" and ", ToODataPredicateList());
        }

        private string OrExpression(PropInfo info)
        {
            var predicates = new List<string>();
            if (info.Type == typeof(int[]))
            {
                predicates.AddRange((info.Value as int[]).Select(x => $"{info.Name.TrimEnd('s')} eq " + x));
            }
            else if (info.Type == typeof(int?))
            {
                var intVal = (int?)info.Value;
                if (intVal.HasValue) predicates.Add($"{info.Name} eq " + intVal.Value);
            }
            return predicates.Any() ? "(" + string.Join($" or ", predicates) + ")" : null;
        }

    }
}
