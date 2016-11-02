using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Data.Entity.Lookups
{
    public abstract class LookupBase : EdubaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public override string ToString() => Name;
    }

    public class AdmissionsPolicy : LookupBase { }
    public class EducationPhase : LookupBase { }
    public class EstablishmentStatus : LookupBase { }
    public class EstablishmentType : LookupBase { }
    public class Gender : LookupBase { }
    public class GroupType : LookupBase { }
    public class HeadTitle : LookupBase { }
    public class ProvisionBoarding : LookupBase { }
    public class ProvisionNursery : LookupBase { }
    public class ProvisionOfficialSixthForm : LookupBase { }
    public class ProvisionSpecialClasses : LookupBase { }
    public class ReasonEstablishmentClosed : LookupBase { }
    public class ReasonEstablishmentOpened : LookupBase { }
    public class ReligiousCharacter : LookupBase { }
    public class ReligiousEthos : LookupBase { }
    public class GovernorRole : LookupBase { }
    public class GovernorAppointingBody : LookupBase { }

}


