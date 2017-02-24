using System;

namespace Edubase.Services.Domain
{

    public class EdubaseChangeDto : ChangeDescriptorDto
    {
        public int Id { get; set; }

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

    public class EstablishmentChangeDto : EdubaseChangeDto
    {
        public int Urn { get; set; }
        
    }

    public class GroupChangeDto : EdubaseChangeDto
    {
        public int GroupUId { get; set; }

    }
}
