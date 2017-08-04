using Edubase.Common;

namespace Edubase.Services.Domain
{
    public class LookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? DisplayOrder { get; set; }
        public string Code { get; set; }
        public int? CodeAsInt => Code.ToInteger();

        public override string ToString() => Name;
    }
}
