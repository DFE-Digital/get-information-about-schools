using System.ComponentModel.DataAnnotations;

namespace Edubase.AcceptanceTests.Authentication
{
    public sealed class DatabaseOptions
    {
        public const string Key = "db";

        [Required(AllowEmptyStrings = false)]
        public string ConnectionString { get; set; } = string.Empty;
    }
}
