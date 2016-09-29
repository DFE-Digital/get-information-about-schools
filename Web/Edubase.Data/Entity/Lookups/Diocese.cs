using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Data.Entity.Lookups
{
    public class Diocese : EdubaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
