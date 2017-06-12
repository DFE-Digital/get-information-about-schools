using Edubase.Services.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Models
{
    public class AmalgamateMergeRequest
    {
        public enum eOperationType
        {
            Merge,
            Amalgamate
        }
        
        [JsonIgnore]
        public eOperationType OperationType { get; set; }

        /// <summary>
        /// Will be the value 'merge' or 'amalgamate' 
        /// </summary>
        [JsonProperty("operationType")]
        public string OperationTypeDescriptor { get; set; }

        /// <summary>
        /// When the operationType is 'merge' this member will be populated with the urn of the establishment to left open.
        /// </summary>
        public int? LeadEstablishmentUrn { get; set; }

        public DateTime? MergeOrAmalgamationDate { get; set; }

        /// <summary>
        /// When the operationType is 'merge'; defines the list of establishments to be merged with the lead establishment.
        /// </summary>
        public int[] UrnsToMerge { get; set; }

        /// <summary>
        /// When the operationType is 'amalgamate'; defines the name of the new establishment.       
        /// </summary>
        public string NewEstablishmentName { get; set; }

        /// <summary>
        /// When the operationType is 'amalgamate'; defines the phase of the new establishment.
        /// </summary>
        public int? NewEstablishmentPhaseId { get; set; }

        /// <summary>
        /// When the operationType is 'amalgamate'; defines the type of the new establishment.
        /// </summary>
        public int? NewEstablishmentTypeId { get; set; }

        /// <summary>
        /// When the operationType is 'amalgamate'; defines the LA of the new establishment.
        /// </summary>
        public int? NewEstablishmentLocalAuthorityId { get; set; }
    }

    public class AmalgamateMergeResult
    {
        /// <summary>
        /// When the amalgamation operation has completed, this contains the URN of the new establishment.
        /// </summary>
        public int? AmalgamateNewEstablishmentUrn { get; set; }
    }

    public class AmalgamateMergeValidationEnvelope : ValidationEnvelopeDto
    {
        public int? Urn { get; set; }
    }
}
