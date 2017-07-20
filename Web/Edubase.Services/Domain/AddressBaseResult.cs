using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class AddressBaseResult
    {
        public string Dataset { get; set; }
        public string UPRN { get; set; }
        public string Address { get; set; }
        public string OrganisationName { get; set; }
        public string BuildingNumber { get; set; }
        public string ThoroughfareName { get; set; }
        public object DependentLocality { get; set; }
        public string PostTown { get; set; }
        public string Postcode { get; set; }
        public string RPC { get; set; }
        public string xCoordinate { get; set; }
        public string yCoordinate { get; set; }
        public string Status { get; set; }
        public string LogicalStatusCode { get; set; }
        public string ClassificationCode { get; set; }
        public string ClassificationCodeDescription { get; set; }
        public string LocalCustodianCode { get; set; }
        public string LocalCustodianCodeDescription { get; set; }
        public string PostalAddressCode { get; set; }
        public string PostalAddressCodeDescription { get; set; }
        public string BLPUStateCode { get; set; }
        public string BLPUStateCodeDescription { get; set; }
        public string TopographyLayerToid { get; set; }
        public string LastUpdateDate { get; set; }
        public string EntryDate { get; set; }
        public string BLPUStateDate { get; set; }
        public string Language { get; set; }
        public double Match { get; set; }
        public string MatchDescription { get; set; }
    }
}
