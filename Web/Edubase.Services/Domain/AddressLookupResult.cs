using Edubase.Common;

namespace Edubase.Services.Domain
{
    public class AddressLookupResult
    {
        private readonly AddressBaseResult _result;

        public AddressLookupResult(AddressBaseResult result)
        {
            _result = result;
        }

        public string Street => StringUtil.ConcatNonEmpties(", ", _result.BuildingNumber, _result.ThoroughfareName);
        public string Town => _result.PostTown;
        public string UPRN => _result.UPRN;
    }
}
