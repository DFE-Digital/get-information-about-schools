using System;
using System.ComponentModel.DataAnnotations;

namespace Edubase.Data.Entity
{
    public abstract class EdubaseEntity
    {
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

    }
}
