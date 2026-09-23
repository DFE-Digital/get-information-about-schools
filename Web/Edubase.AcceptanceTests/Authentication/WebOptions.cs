using System.ComponentModel.DataAnnotations;

namespace Edubase.AcceptanceTests.Authentication
{
    public sealed class WebOptions
    {
        public const string Key = "web";
        private const ushort DEFAULT_HTTPS_PORT = 443;

        public string Scheme { get; set; } = "https";

        [Required(AllowEmptyStrings = false)]
        public string Domain { get; set; } = default!;
        public ushort Port { get; set; } = DEFAULT_HTTPS_PORT;
        public bool BasicAuthEnabled { get; set; } = false;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string WebUri => $"{Scheme}://{Domain}:{Port}";
        public string WebUriWithBasicAuth => $"{Scheme}://{Username}:{Password}@{Domain}:{Port}";
    }
}
