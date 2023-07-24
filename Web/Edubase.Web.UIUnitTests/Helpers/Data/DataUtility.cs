using System.Collections.Generic;
using Edubase.Services.Domain;

namespace Edubase.Web.UIUnitTests.Helpers.Data
{
    public static class DataUtility
    {
        public static IEnumerable<EstablishmentLookupDto> GetEstablishmentLookupDto()
        {
            return new List<EstablishmentLookupDto>()
            {
                new EstablishmentLookupDto() { Id = 46, Code = "57", Name = "Secure 16-19 Academy 1"},
                new EstablishmentLookupDto() { Id = 39, Code = "45", Name = "Academy 16-19 Converter 1"},
                new EstablishmentLookupDto() { Id = 38, Code = "44", Name = "Academy Special Converter"},
                new EstablishmentLookupDto() { Id = 46, Code = "57",Name = "Secure 16-19 Academy 2"},
                new EstablishmentLookupDto() { Id = 39, Code = "45", Name = "Academy 16-19 Converter 2"},
                new EstablishmentLookupDto() { Id = 38, Code = "44",Name = "Academy Special Converter"}
            };
        }
    }
}
