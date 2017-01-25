using System;
using System.Threading.Tasks;
using Edubase.Services.Domain;

namespace Edubase.Services.Core
{
    public interface IFileDownloadFactoryService
    {
        Task<SearchDownloadGenerationProgressDto> GetDownloadGenerationProgressAsync(Guid taskId);
        Task<SearchDownloadGenerationProgressDto> SearchWithDownloadGeneration_InitialiseAsync();
    }
}