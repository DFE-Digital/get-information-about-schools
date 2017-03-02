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
    public class NomenclatureService
    {
        public string GetGovernorRoleName(eLookupGovernorRole role, eTextCase textCase = eTextCase.SentenceCase) => role.ToString().ToProperCase().ToTextCase(textCase);
    }
}
