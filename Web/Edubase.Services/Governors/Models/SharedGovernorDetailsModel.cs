namespace Edubase.Services.Governors.Models
{
    using System.Collections.Generic;

    public class SharedGovernorDetailsModel : GovernorModel
    {
        public IEnumerable<GovernorAppointment> Appointments { get; set; }
    }
}
