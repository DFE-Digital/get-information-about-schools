using Edubase.Common;
using Edubase.Services.Governors.Models;
using Edubase.Services.IntegrationEndPoints.AzureSearch;
using System;

namespace Edubase.Services.Governors.Search
{
    public class SearchGovernorDocument : GovernorModelBase
    {
        [AZSIgnore]
        public string EstablishmentName { get; set; }
        [AZSIgnore]
        public string GroupName { get; set; }
    }
}
