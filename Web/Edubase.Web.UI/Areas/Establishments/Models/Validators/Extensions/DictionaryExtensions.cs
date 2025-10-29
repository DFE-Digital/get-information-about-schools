using System.Collections.Generic;
using System.Linq;
using Edubase.Services.Enums;

namespace Edubase.Web.UI.Areas.Establishments.Models.Validators.Extensions;

public static class DictionaryExtensions
{
    public static Dictionary<int, List<int>> AsInts(
        this Dictionary<eLookupEstablishmentType, eLookupEducationPhase[]> source) =>
            source.ToDictionary(
                kvp => (int) kvp.Key,
                kvp => kvp.Value.Select(eLookupEducationPhase => (int)eLookupEducationPhase).ToList()
            );
}
