using Edubase.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class ToolsViewModel
    {
        public bool UserCanCreateChildrensCentreGroup { get; set; }
        public bool UserCanCreateAcademyTrustGroup { get; set; }
        public bool UserCanCreateFederationGroup { get; set; }
        public bool UserCanCreateSchoolTrustGroup { get; set; }

        public bool UserCanCreateAtLeastOneGroupType => UserCanCreateChildrensCentreGroup || UserCanCreateAcademyTrustGroup || UserCanCreateFederationGroup || UserCanCreateSchoolTrustGroup;

    }
}