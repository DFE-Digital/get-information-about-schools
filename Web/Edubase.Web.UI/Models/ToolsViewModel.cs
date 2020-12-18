using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Edubase.Web.UI.Models
{
    public class ToolsViewModel
    {
        public class LinkAction
        {
            public MvcHtmlString Link { get; set; }
            public string Description { get; set; }
        }

        public bool UserCanCreateChildrensCentreGroup { get; set; }
        public bool UserCanCreateAcademyTrustGroup { get; set; }
        public bool UserCanCreateFederationGroup { get; set; }
        public bool UserCanCreateSchoolTrustGroup { get; set; }
        public bool UserCanCreateAcademySponsor { get; set; }

        public bool UserCanCreateEstablishment { get; set; }

        public bool UserCanManageAcademyOpenings { get; set; }
        public bool UserCanMergeOrAmalgamateEstablishments { get; internal set; }
        public bool UserCanBulkCreateAcademies { get; internal set; }
        public bool UserCanBulkUpdateGovernors { get; internal set; }
        public bool UserCanBulkUpdateEstablishments { get; internal set; }
        public bool UserCanApprove { get; internal set; }
        public bool UserCanConvertAcademyTrusts { get; internal set; }
        public bool UserCanViewIndependentSchoolsSignificantDates { get; internal set; }
        public bool UserCanBulkCreateFreeSchools { get; internal set; }
        public bool UserCanBulkAssociateEstabs2Groups { get; internal set; }
        public bool UserCanDownloadMATClosureReport { get; internal set; }

        public List<LinkAction> GetCreateActions(HtmlHelper htmlHelper)
        {
            var retVal = new List<LinkAction>();

            if (UserCanCreateEstablishment) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("Create new establishments", "CreateEstablishment"), Description = "Set up a new establishment record." });
            if (UserCanBulkCreateAcademies) retVal.Add(new LinkAction { Link = htmlHelper.ActionLink("Bulk create new academies", "BulkAcademies", "Tools"), Description = "Bulk create new academy records collectively and not individually." });
            if (UserCanBulkCreateFreeSchools) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("Bulk create new free schools", "BulkCreateFreeSchools"), Description = "Bulk create new free school records collectively rather than individually." });
            if (UserCanMergeOrAmalgamateEstablishments) retVal.Add(new LinkAction { Link = htmlHelper.ActionLink("Amalgamate or merge establishments", "MergersTool", "Tools"), Description = "Carry out an amalgamation or merger for establishments." });
            if (UserCanCreateChildrensCentreGroup) retVal.Add(new LinkAction { Link = htmlHelper.ActionLink("Create new children's centre groups or children's centre collaborations", "CreateNewGroup", "Group", new { area = "Groups", type = "ChildrensCentre" }, null), Description = "Set up a new children's centre group or children's centre collaboration record." });
            if (UserCanCreateFederationGroup) retVal.Add(new LinkAction { Link = htmlHelper.ActionLink("Create new federations", "CreateNewGroup", "Group", new { area = "Groups", type = "Federation" }, null), Description = "Set up a new federation record." });
            if (UserCanCreateSchoolTrustGroup) retVal.Add(new LinkAction { Link = htmlHelper.ActionLink("Create new trusts", "CreateNewGroup", "Group", new { area = "Groups", type = "Trust" }, null), Description = "Set up a new trust record." });
            if (UserCanCreateAcademySponsor) retVal.Add(new LinkAction { Link = htmlHelper.ActionLink("Create new academy sponsors", "CreateNewGroup", "Group", new { area = "Groups", type = "Sponsor" }, null), Description = "Set up a new academy sponsor record." });
            if (UserCanCreateAcademyTrustGroup) retVal.Add(new LinkAction { Link = htmlHelper.ActionLink("Create new academy trusts", "SearchCompaniesHouse", "Group", new { area = "Groups" }, null), Description = "Set up a new academy trust record." });
            if (UserCanConvertAcademyTrusts) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("Convert a single academy trust", "GroupConvertSAT2MAT"), Description = "Convert a single academy trust (SAT) to a multi academy trust (MAT)." });

            return retVal;
        }

        public List<LinkAction> GetUpdateActions(HtmlHelper htmlHelper)
        {
            var retVal = new List<LinkAction>();
            if (UserCanBulkUpdateEstablishments) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("Bulk update establishments", "EstabBulkUpdate"), Description = "Bulk update establishment data collectively rather than individually." });
            if (UserCanBulkUpdateGovernors) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("Bulk update governance information", "GovernorsBulkUpdate"), Description = "Bulk update governance information collectively rather than individually." });
            if (UserCanBulkAssociateEstabs2Groups) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("Bulk upload academies to academy sponsors and<span class=\"govuk-visually-hidden\">&nbsp;</span>/<span class=\"govuk-visually-hidden\">&nbsp;</span>or academy trusts", "BulkAssociateEstabs2Groups"), Description = "Bulk upload academies to academy sponsors and<span class=\"visually-hidden\">&nbsp;</span>/<span class=\"visually-hidden\">&nbsp;</span>or academy trusts collectively rather than individually." });
            return retVal;
        }

        public List<LinkAction> GetAdminActions(HtmlHelper htmlHelper)
        {
            var retVal = new List<LinkAction>();

            retVal.Add(new LinkAction { Link = htmlHelper.ActionLink("View data status", "ViewStatus", "DataQuality"), Description = "See when each team's data was last updated.<br />Data owner teams can also confirm their team's data is up to date." });
            if (UserCanApprove) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("Manage change requests", "PendingApprovals"), Description = "Review your pending change requests and approve or reject." });
            if (UserCanManageAcademyOpenings) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("Manage academy openings", "ManageAcademyOpenings"), Description = "View details of proposed-to-open academies.<br />Edit academy names and opening dates." });
            if (UserCanViewIndependentSchoolsSignificantDates) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("View independent schools' next significant action dates", "IndSchSearch"), Description = "View independent schools' next general action required dates and next action required by welfare dates." });
            if (UserCanDownloadMATClosureReport) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("View closed Companies House single academy trusts and multi academy trusts", "DownloadMATClosureReport"), Description = "View a list of closed single academy trusts and multi-academy trusts currently open on GIAS but reported closed by Companies House." });
            retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("View establishment and group change history", "ChangeHistoryCriteria"), Description = "View establishment and group data changes." });

            return retVal;
        }




    }
}
