namespace Edubase.Services.IntegrationEndPoints.OSPlaces.Models
{
    public class OSPlacesResponse
    {
        public Header Header { get; set; }
        public Result[] Results { get; set; }
    }

    public class Header
    {
        public string Uri { get; set; }
        public string Query { get; set; }
        public long Offset { get; set; }
        public long Totalresults { get; set; }
        public string Format { get; set; }
        public string Dataset { get; set; }
        public string Lr { get; set; }
        public long Maxresults { get; set; }
        public long Epoch { get; set; }
        public string OutputSrs { get; set; }
    }

    public class Result
    {
        public OSAddress Dpa { get; set; }
    }

    public class OSAddress
    {
        public string Uprn { get; set; }
        public long Udprn { get; set; }
        public string Address { get; set; }
        public string OrganisationName { get; set; }
        public long BuildingNumber { get; set; }
        public string ThoroughfareName { get; set; }
        public string DependentLocality { get; set; }
        public string PostTown { get; set; }
        public string Postcode { get; set; }
        public long Rpc { get; set; }
        public long XCoordinate { get; set; }
        public long YCoordinate { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }
        public string Status { get; set; }
        public long LogicalStatusCode { get; set; }
        public string ClassificationCode { get; set; }
        public string ClassificationCodeDescription { get; set; }
        public long LocalCustodianCode { get; set; }
        public string LocalCustodianCodeDescription { get; set; }
        public string PostalAddressCode { get; set; }
        public string PostalAddressCodeDescription { get; set; }
        public long BlpuStateCode { get; set; }
        public string BlpuStateCodeDescription { get; set; }
        public string TopographyLayerToid { get; set; }
        public string LastUpdateDate { get; set; }
        public string EntryDate { get; set; }
        public string BlpuStateDate { get; set; }
        public string Language { get; set; }
        public long Match { get; set; }
        public string MatchDescription { get; set; }
    }
}
