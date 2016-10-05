using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Data.Entity
{
    public class Estab2Estab
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Establishment Establishment { get; set; }
        public Establishment LinkedEstablishment { get; set; }
        public string LinkName { get; set; }
        public string LinkType { get; set; }
        public DateTime? LinkEstablishedDate { get; set; }
    }
}
