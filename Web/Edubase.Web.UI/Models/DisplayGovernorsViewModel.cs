using System;
using System.Collections.Generic;
using Edubase.Services.Governors.Models;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Models.Grid;

namespace Edubase.Web.UI.Models;

public class DisplayGovernorsViewModel
{
    public GovernorsDetailsDto DomainModel { get; set; } = default!;
    public GovernorPermissions GovernorPermissions { get; set; } = default!;
    public List<GovernorGridViewModel> Grids { get; set; } = [];
    public List<GovernorGridViewModel> HistoricGrids { get; set; } = [];
    public List<HistoricGovernorViewModel> HistoricGovernors { get; set; } = [];
    public bool EditMode { get; set; }
    public int? RemovalGid { get; set; }
    public int? EstablishmentUrn { get; set; }
    public int? GroupUId { get; set; }
    public bool? GovernorShared { get; set; }
    public DateTime? RemovalAppointmentEndDate { get; set; }
}

