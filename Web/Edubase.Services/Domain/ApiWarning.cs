namespace Edubase.Services.Domain
{
    public class ApiWarning
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Fields { get; set; }
        public string[] MessageParameters { get; set; }
    }
}
