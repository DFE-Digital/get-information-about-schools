using System.ComponentModel.DataAnnotations;

namespace Edubase.AcceptanceTests.Authentication
{
    public sealed class BackendOptions
    {
        public const string Key = "backend";

        [Required(AllowEmptyStrings = false)]
        public string Scheme { get; set; } = "https";
        public string Domain { get; set; } = default!;
        public string WebUri => $"{Scheme}://{Domain}";
        public string EdubaseVersion { get; set; }
    }
}
