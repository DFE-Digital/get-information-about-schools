using Edubase.Common;
using Edubase.Common.Text;
using Edubase.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Nomenclature
{
    using GT = Edubase.Services.Enums.eLookupGroupType;

    public class NomenclatureService
    {
        public string GetGovernorRoleName(eLookupGovernorRole role, eTextCase textCase = eTextCase.SentenceCase) => role.ToString().ToProperCase().ToTextCase(textCase);

        public string GetEstablishmentsPluralName(GT groupType, eTextCase textCase = eTextCase.TitleCase)
        {
            if (groupType.OneOfThese(GT.ChildrensCentresCollaboration, GT.ChildrensCentresGroup)) return "children's centres".ToTextCase(textCase);
            else if (groupType.OneOfThese(GT.Federation, GT.Trust)) return "schools".ToTextCase(textCase);
            else if (groupType.OneOfThese(GT.MultiacademyTrust, GT.SingleacademyTrust, GT.SchoolSponsor)) return "academies".ToTextCase(textCase);
            else throw new NotImplementedException($"Group type '{groupType}' is not supported for this operation");
        }
    }
}
