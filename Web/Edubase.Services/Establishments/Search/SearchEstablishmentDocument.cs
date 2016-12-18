using Edubase.Common;
using System;

namespace Edubase.Services.Establishments.Search
{
    public class SearchEstablishmentDocument : SearchEstablishmentDocumentBase
    {
        public string GetAddress() => StringUtil.ConcatNonEmpties(", ", Address_Line1, Address_Line2, Address_Line3, Address_Locality, Address_CityOrTown, Address_County, Address_PostCode);

        public string GetLAESTAB() => string.Concat(LocalAuthorityId, EstablishmentNumber.GetValueOrDefault().ToString("D4"));
    }
}
