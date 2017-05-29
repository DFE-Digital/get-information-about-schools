using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Edubase.Services.Texuna.ChangeHistory.Models;

namespace Edubase.Web.UI.Models
{
    public class ChangeHistoryViewModel
    {
        public List<EstablishmentField> EstablishmentFields { get; internal set; }
    }
}