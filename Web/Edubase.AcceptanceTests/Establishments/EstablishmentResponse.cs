namespace Edubase.AcceptanceTests.Establishments
{
    public partial class EstablishmentsStepDefinitions
    {
        public class EstablishmentResponse
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
}
