using Edubase.Common;
using Edubase.Common.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Search
{
    public class EstablishmentSearchFilters
    {
        public int[] LocalAuthorityIds { get; set; } = new int[0];

        public int[] StatusIds { get; set; } = new int[0];

        public int[] ReasonEstablishmentOpenedIds { get; set; } = new int[0];

        public int[] ReasonEstablishmentClosedIds { get; set; } = new int[0];

        public int[] EducationPhaseIds { get; set; } = new int[0];

        public int[] ProvisionBoardingIds { get; set; } = new int[0];

        public int[] ProvisionNurseryIds { get; set; } = new int[0];

        public int[] ProvisionOfficialSixthFormIds { get; set; } = new int[0];

        public int[] GenderIds { get; set; } = new int[0];

        public int[] ReligiousCharacterIds { get; set; } = new int[0];

        public int[] ReligiousEthosIds { get; set; } = new int[0];

        public int[] DioceseIds { get; set; } = new int[0];

        public int[] AdmissionsPolicyIds { get; set; } = new int[0];

        public int[] ProvisionSpecialClassesIds { get; set; } = new int[0];

        public int[] HeadTitleIds { get; set; } = new int[0];

        public int[] TypeIds { get; set; } = new int[0];

        public int[] EstablishmentTypeGroupIds { get; set; } = new int[0];

        public int[] InspectorateIds { get; set; } = new int[0];

        public int[] Section41ApprovedIds { get; set; } = new int[0];

        public int[] SEN1Ids { get; set; } = new int[0];

        public int[] SEN2Ids { get; set; } = new int[0];

        public int[] SEN3Ids { get; set; } = new int[0];

        public int[] SEN4Ids { get; set; } = new int[0];

        public int[] TeenageMothersProvisionIds { get; set; } = new int[0];

        public int[] ChildcareFacilitiesIds { get; set; } = new int[0];

        public int[] PRUSENIds { get; set; } = new int[0];

        public int[] PRUEBDIds { get; set; } = new int[0];

        public int[] PruFulltimeProvisionIds { get; set; } = new int[0];

        public int[] PruEducatedByOthersIds { get; set; } = new int[0];

        public int[] TypeOfResourcedProvisionIds { get; set; } = new int[0];

        public int[] GovernmentOfficeRegionIds { get; set; } = new int[0];

        public int[] AdministrativeDistrictIds { get; set; } = new int[0];

        public int[] AdministrativeWardIds { get; set; } = new int[0];

        public int[] ParliamentaryConstituencyIds { get; set; } = new int[0];

        public int[] UrbanRuralIds { get; set; } = new int[0];

        public int[] GSSLAIds { get; set; } = new int[0];

        public int[] CASWardIds { get; set; } = new int[0];

        public int[] MSOAIds { get; set; } = new int[0];

        public int[] LSOAIds { get; set; } = new int[0];

        public int[] FurtherEducationTypeIds { get; set; } = new int[0];

        public int[] RSCRegionIds { get; set; } = new int[0];

        public int[] BSOInspectorateIds { get; set; } = new int[0];

        public int? UKPRN { get; set; }
        public int? EstablishmentNumber { get; set; }

        /// <summary>
        /// Returns an OData filter expression
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var props = ReflectionHelper.GetProperties(this);
            var predicates = props.Select(x => OrExpression(ReflectionHelper.GetPropertyInfo(this, x))).Where(x => !x.IsNullOrEmpty());
            return string.Join(" and ", predicates);
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
