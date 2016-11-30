using System.ComponentModel.DataAnnotations;

namespace Edubase.Data.Entity
{
    public class LocalAuthority : EdubaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public int Order { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }

        public override string ToString() => Name;
    }
}
