using System;
using Edubase.Services.Governors.Models;
using Edubase.Web.UI.Models.Grid;

namespace Edubase.Web.UI.Models;

public class GovernorEditorViewModel
{
    public GovernorGridViewModel Grid { get; set; } = default!;
    public bool Historic { get; set; }
    public bool EditMode { get; set; }
    public int? RemovalGid { get; set; }
    public int? EstablishmentUrn { get; set; }
    public int? GroupUId { get; set; }
    public bool? GovernorShared { get; set; }
    public GovernorPermissions GovernorPermissions { get; set; } = default!;
    public DateTime? RemovalAppointmentEndDate { get; set; }
}
