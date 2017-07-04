using System.Collections.Generic;
using System.Linq;

namespace Edubase.Services.Domain
{
    public class ValidationEnvelopeDto
    {
        public List<ApiWarning> Warnings { get; set; }

        public List<ApiError> Errors { get; set; }

        public bool HasErrors => Errors != null && Errors.Any();

        public bool HasWarnings => Warnings != null && Warnings.Any();
    }
}
