using Edubase.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class ToolsViewModel
    {
        public bool UserCanCreateEstablishment { get; internal set; }
        
        public eLookupGroupType[] UserCanCreateGroupsOfTypes { get; internal  set; }
    }
}