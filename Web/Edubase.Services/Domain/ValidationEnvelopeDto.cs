using System.Collections.Generic;
using System.Linq;

namespace Edubase.Services.Domain
{
    public class ValidationEnvelopeDto
    {
        public List<ApiWarning> Warnings { get; set; } = new List<ApiWarning>();

        public List<ApiError> Errors { get; set; } = new List<ApiError>();

        public bool HasErrors => Errors != null && Errors.Any();

        public bool HasWarnings => Warnings != null && Warnings.Any();
    }
}
