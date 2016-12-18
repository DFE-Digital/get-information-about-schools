using Edubase.Services.Establishments.Enums;
using System.Collections.Generic;

namespace Edubase.Services.Establishments
{
    public interface ILAESTABService
    {
        int GenerateEstablishmentNumber(int establishmentTypeId, int educationPhaseId, int localAuthorityId);
        EstabNumberEntryPolicy GetEstabNumberEntryPolicy(int establishmentTypeId, int educationPhaseId);
        Dictionary<string, string> GetSimplifiedRules();
    }
}