using System;

namespace Edubase.Services.Domain
{

    public class EdubaseChangeDto : ChangeDescriptorDto
    {
        public new string  Id { get; set; }
        public string OriginatorUserName { get; set; }
        public string ApproverUserName { get; set; }
        public DateTime? EffectiveDateUtc { get; set; }
        public DateTime? RequestedDateUtc { get; set; }
        public DateTime? ChangedDateUtc { get; set; }
    }

    public class EstablishmentChangeDto : EdubaseChangeDto
    {
        public int Urn { get; set; }
        
    }
}
