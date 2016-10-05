using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Data.Entity
{
    public class Establishment2Company
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Establishment Establishment { get; set; }

        public Company Company { get; set; }
        public DateTime? JoinedDate { get; set; }
    }
}
