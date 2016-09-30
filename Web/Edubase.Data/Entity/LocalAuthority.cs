using System.ComponentModel.DataAnnotations;

namespace Edubase.Data.Entity
{
    public class LocalAuthority
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public int Order { get; set; }

        public override string ToString() => Name;
    }
}
