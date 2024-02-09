using System;
using System.ComponentModel;
using Edubase.Common;
using Edubase.Common.Text;
using Edubase.Services.Enums;
using Humanizer;

namespace Edubase.Services.Nomenclature
{
    using GT = eLookupGroupType;

    public class NomenclatureService
    {
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
                case GT.SecureSingleAcademyTrust:
                    return "academies".ToTextCase(textCase);
                case GT.UmbrellaTrust:
                default:
                    throw new InvalidEnumArgumentException(nameof(groupType),
                        (int) groupType,
                        groupType.GetType());
            }
        }
    }
}
