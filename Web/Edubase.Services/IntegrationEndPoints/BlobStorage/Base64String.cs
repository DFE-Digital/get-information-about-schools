namespace Edubase.Services.IntegrationEndPoints.BlobStorage
{
    public class Base64String
    {
        public string Data { get; set; }

        public Base64String(string data)
        {
            Data = data;
        }

        public override string ToString() =>Data;
    }
}
