using Edubase.Common;
using Newtonsoft.Json;

namespace Edubase.Services.Governors.Search
{
    public class GovernorSuggestionItem
    {
        public int Id { get; set; }
        public string Text => StringUtil.ConcatNonEmpties(Person_FirstName, Person_MiddleName, Person_LastName);
        public string Person_FirstName { get; set; }
        public string Person_MiddleName { get; set; }
        public string Person_LastName { get; set; }
    }
}
