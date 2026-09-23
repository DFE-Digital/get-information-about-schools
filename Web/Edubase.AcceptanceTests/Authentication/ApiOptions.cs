using System.ComponentModel.DataAnnotations;

namespace Edubase.AcceptanceTests.Authentication
{
    public sealed class ApiOptions
    {
        public const string Key = "api";

        public string Scheme { get; set; } = "https";

        [Required(AllowEmptyStrings = false)]
        public string Domain { get; set; } = default!;

        public ushort Port { get; set; } = 443;

        public bool AuthEnabled { get; set; } = false;

        public string AuthUsername { get; set; } = string.Empty;

        public string AuthPassword { get; set; } = string.Empty;
        //TODO interpolate in port, current domain has /rest appended so not simple interpolation
        public string Url => $"{Scheme}://{Domain.TrimEnd('/')}";
    }
}
