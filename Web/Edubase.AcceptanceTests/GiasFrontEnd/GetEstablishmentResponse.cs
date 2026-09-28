namespace Edubase.AcceptanceTests.GiasFrontEnd
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
