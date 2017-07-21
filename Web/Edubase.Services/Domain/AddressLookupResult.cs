using Edubase.Common;

namespace Edubase.Services.Domain
{
    public class AddressLookupResult
    {
        public AddressLookupResult(AddressBaseResult result)
        {
            Street = StringUtil.ConcatNonEmpties(" ", result.BuildingNumber, result.ThoroughfareName);
            Town = result.PostTown;
            UPRN = result.UPRN;
            PostCode = result.Postcode;
        }

        public AddressLookupResult()
        {

        }

        public string Street { get; set; }
        public string Town { get; set; }
        public string UPRN { get; set; }
        public string PostCode { get; set; }

        public override string ToString() => Street + ", " + Town;
    }
}
