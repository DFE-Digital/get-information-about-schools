using System.Collections.Generic;

namespace Edubase.Services.Domain
{
    public class ValidationEnvelopeDto
    {
        public List<ApiWarning> Warnings { get; set; }

        public List<ApiError> Errors { get; set; }
    }
}
