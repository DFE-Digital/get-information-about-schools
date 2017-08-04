using Edubase.Services.Domain;
using Edubase.Services.Governors.Search;

namespace Edubase.Services.Governors.Downloads
{
    public class GovernorSearchDownloadPayload : SearchDownloadDto<GovernorSearchPayload>
    {
        public bool IncludeNonPublicData { get; set; }
    }
}
