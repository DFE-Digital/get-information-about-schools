using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    public interface IGroupPageViewModel
    {
        string PageTitle { get; }
        int? GroupUId { get; }
        string ListOfEstablishmentsPluralName { get; }
        string GroupName { get; }
        int? GroupTypeId { get; }
    }
}