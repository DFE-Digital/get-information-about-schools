using System;

namespace Edubase.Data.Entity
{
    public abstract class EdubaseEntity
    {
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public bool IsDeleted { get; set; }
    }
}
