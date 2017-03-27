using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models.Establishments
{
    public interface IEstablishmentPageViewModel
    {
        string SelectedTab { get; set; }
        int? Urn { get; set; }
        string Name { get; set; }
        TabDisplayPolicy TabDisplayPolicy { get; set; }
        string Layout { get; set; }
    }
}