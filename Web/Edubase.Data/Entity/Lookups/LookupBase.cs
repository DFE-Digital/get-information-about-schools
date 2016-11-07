using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Data.Entity.Lookups
{
    public abstract class LookupBase : EdubaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public short? DisplayOrder { get; set; }
        
        [Obsolete("Contains the old 'Code' primary key value.  Code values are deprecated.")]
        public string Code { get; set; }
        
        public override string ToString() => Name;
        
    }
    
    
}


