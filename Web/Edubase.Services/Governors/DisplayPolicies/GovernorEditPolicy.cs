namespace Edubase.Services.Governors.DisplayPolicies
{
    public class GovernorEditPolicy
    {
        public bool Id { get; set; }
        public bool FullName { get; set; }
        public bool AppointmentStartDate { get; set; }
        public bool AppointmentEndDate { get; set; }
        public bool RoleId { get; set; } = true;
        public bool AppointingBodyId { get; set; }
        public bool EmailAddress { get; set; }
        public bool DOB { get; set; }
        public bool PostCode { get; set; }
        public bool PreviousFullName { get; set; }
        public bool TelephoneNumber { get; set; }

        public GovernorEditPolicy Clone() => MemberwiseClone() as GovernorEditPolicy;

    }
}
