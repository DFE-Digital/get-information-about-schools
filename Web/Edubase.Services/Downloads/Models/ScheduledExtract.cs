using System;

namespace Edubase.Services.Downloads.Models
{
    public class ScheduledExtract
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public DateTime? Date { get; set; }
    }
}
