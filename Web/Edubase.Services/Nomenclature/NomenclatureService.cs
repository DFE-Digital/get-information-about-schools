using Edubase.Common;
using Edubase.Common.Text;
using Edubase.Services.Enums;
using System;
using Humanizer;

namespace Edubase.Services.Nomenclature
{
    using GT = eLookupGroupType;

    public class NomenclatureService
    {
        public string GetGovernorRoleName(eLookupGovernorRole role, eTextCase textCase = eTextCase.SentenceCase, bool pluralise = false)
        {
            var roleName =  role.ToString();
            if (roleName.Contains("_"))
            {
                var index = roleName.IndexOf("_", StringComparison.Ordinal);
                roleName = roleName.Substring(index + 1);
            }
            var name = roleName.ToProperCase().ToTextCase(textCase);
            if (pluralise)
            {
                name = name.Pluralize();
            }

            return name;
        }

        public string GetEstablishmentsPluralName(GT groupType, eTextCase textCase = eTextCase.TitleCase)
        {
            if (groupType.OneOfThese(GT.ChildrensCentresCollaboration, GT.ChildrensCentresGroup)) return "children's centres".ToTextCase(textCase);
            else if (groupType.OneOfThese(GT.Federation, GT.Trust)) return "schools".ToTextCase(textCase);
            else if (groupType.OneOfThese(GT.MultiacademyTrust, GT.SingleacademyTrust, GT.SchoolSponsor)) return "academies".ToTextCase(textCase);
            else throw new NotImplementedException($"Group type '{groupType}' is not supported for this operation");
        }
    }
}
