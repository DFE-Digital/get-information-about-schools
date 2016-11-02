using System;

namespace Edubase.Services.Domain
{
    public class EstablishmentChangeDto
    {
        public int Id { get; set; }
        public int Urn { get; set; }
        public string Name { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public string OriginatorUserId { get; set; }
        public string OriginatorUserName { get; set; }
        public string ApproverUserName { get; set; }
        public string ApproverUserId { get; set; }
        
        /// <summary>
        /// Date the change actually occurred
        /// </summary>
        public DateTime? EffectiveDateUtc { get; set; }

        /// <summary>
        /// Date the change was first requested/done
        /// </summary>
        public DateTime? RequestedDateUtc { get; set; }
    }
}
