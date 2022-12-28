using System;
using Edubase.Common;
using Edubase.Common.Text;
using Edubase.Services.Enums;
using Humanizer;

namespace Edubase.Services.Nomenclature
{
    using GT = eLookupGroupType;

    public class NomenclatureService
    {
        public string GetGovernorRoleName(eLookupGovernorRole role, eTextCase textCase = eTextCase.SentenceCase,
            bool pluralise = false)
        {
            var roleName = role.ToString();
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

        public static string GetEstablishmentsPluralName(GT groupType, eTextCase textCase = eTextCase.TitleCase)
        {
            switch (groupType)
            {
                case GT.ChildrensCentresCollaboration:
                case GT.ChildrensCentresGroup:
                    return "children's centres".ToTextCase(textCase);
                case GT.Federation:
                case GT.Trust:
                    return "schools".ToTextCase(textCase);
                case GT.MultiacademyTrust:
                case GT.SingleacademyTrust:
                case GT.SchoolSponsor:
                    return "academies".ToTextCase(textCase);
                case GT.UmbrellaTrust:
                default:
                    throw new ArgumentOutOfRangeException(nameof(groupType), groupType,
                        $"Group type '{groupType}' is not supported for this operation");
            }
        }
    }
}
