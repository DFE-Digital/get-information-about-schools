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
        public bool UserCanCreateAcademySponsor { get; set; }
        public bool UserCanCreateAtLeastOneGroupType => UserCanCreateChildrensCentreGroup || UserCanCreateAcademyTrustGroup || UserCanCreateFederationGroup || UserCanCreateSchoolTrustGroup || UserCanCreateAcademySponsor;


        public bool UserCanCreateEstablishment { get; set; }

        public bool UserCanManageAcademyOpenings { get; set; }
        public bool UserCanMergeOrAmalgamateEstablishments { get; internal set; }
        public bool UserCanBulkCreateAcademies { get; internal set; }
        public bool UserCanBulkUpdateGovernors { get; internal set; }
        public bool UserCanBulkUpdateEstablishments { get; internal set; }
        public bool UserCanApprove { get; internal set; }
        public bool UserCanSearchChangeHistory { get; internal set; }
    }
}