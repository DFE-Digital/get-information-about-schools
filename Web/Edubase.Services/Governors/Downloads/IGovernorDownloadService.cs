﻿using Edubase.Services.Domain;
using Edubase.Services.Establishments.Downloads;
using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Governors.Downloads
{
    public interface IGovernorDownloadService
    {
        Task<ProgressDto> GetDownloadGenerationProgressAsync(Guid taskId, IPrincipal principal);
        
        Task<Guid> SearchWithDownloadGenerationAsync(GovernorSearchDownloadPayload payload, IPrincipal principal);
        
    }
}