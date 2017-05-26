﻿using System;

namespace Edubase.Services.Domain
{

    public class EdubaseChangeDto : ChangeDescriptorDto
    {
        public string Id { get; set; }
        public string OriginatorUserName { get; set; }
        public string ApproverUserName { get; set; }
        public DateTime? EffectiveDateUtc { get; set; }
        public DateTime? RequestedDateUtc { get; set; }
    }

    public class EstablishmentChangeDto : EdubaseChangeDto
    {
        public int Urn { get; set; }
        
    }
}
