namespace Edubase.Services.Domain
{
    public class ApiError
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Fields { get; set; }

        public bool IsEmpty => Code == null && Message == null && Fields == null;
    }
}
