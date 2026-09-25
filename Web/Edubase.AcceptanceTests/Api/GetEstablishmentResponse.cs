namespace Edubase.AcceptanceTests.Api
{
    public class GetEstablishmentResponse
    {
        public string status;

        public class ReturnValue
        {
            public string name;
            public string typeName;
            public string urn;
        }

        public ReturnValue returnValue;
    }
}
