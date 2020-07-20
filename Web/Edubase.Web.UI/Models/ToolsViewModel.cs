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

            if (UserCanConvertAcademyTrusts) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("Convert academy trusts", "GroupConvertSAT2MAT"), Description = "Convert a single-academy trust into a multi-academy trust." });
            if (UserCanCreateAcademyTrustGroup) retVal.Add(new LinkAction { Link = htmlHelper.ActionLink("Create new academy trust", "SearchCompaniesHouse", "Group", new { area = "Groups" }, null), Description = "Set up a new record for your school trust." });
            if (UserCanCreateEstablishment) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("Create a new establishment", "CreateEstablishment"), Description = "Set up a new establishment record." });
            if (UserCanCreateSchoolTrustGroup) retVal.Add(new LinkAction { Link = htmlHelper.ActionLink("Create new school trust", "CreateNewGroup", "Group", new { area = "Groups", type = "Trust" }, null), Description = "Enter the trust's name and select the opening date and status." });
            if (UserCanMergeOrAmalgamateEstablishments) retVal.Add(new LinkAction { Link = htmlHelper.ActionLink("Merge & amalgamate establishments", "MergersTool", "Tools"), Description = "Carry out mergers and amalgamations." });

            if (UserCanBulkCreateAcademies) retVal.Add(new LinkAction { Link = htmlHelper.ActionLink("Bulk create new academies", "BulkAcademies", "Tools"), Description = "Create records for multi-academy and single-academy trusts using information from companies house." });
            if (UserCanCreateChildrensCentreGroup) retVal.Add(new LinkAction { Link = htmlHelper.ActionLink("Create new children's centre group or collaboration", "CreateNewGroup", "Group", new { area = "Groups", type = "ChildrensCentre" }, null), Description = "Create records for children's centre groups and children's centre collaborations for your local authority." });
            if (UserCanCreateFederationGroup) retVal.Add(new LinkAction { Link = htmlHelper.ActionLink("Create new federation", "CreateNewGroup", "Group", new { area = "Groups", type = "Federation" }, null), Description = "Enter the federation's name and select its status and opening date." });
            if (UserCanCreateAcademySponsor) retVal.Add(new LinkAction { Link = htmlHelper.ActionLink("Create new sponsor", "CreateNewGroup", "Group", new { area = "Groups", type = "Sponsor" }, null), Description = "Enter the sponsor's name and select the open date and status." });
            if (UserCanBulkCreateFreeSchools) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("Bulk create new free schools", "BulkCreateFreeSchools"), Description = "Set up a new free schools records as a collection, rather than by individual entries." });
            return retVal;
        }

        public List<LinkAction> GetUpdateActions(HtmlHelper htmlHelper)
        {
            var retVal = new List<LinkAction>();
            if (UserCanBulkUpdateEstablishments) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("Bulk update establishments", "EstabBulkUpdate"), Description = "Update establishment data as a collection, rather than by individual entries." });
            if (UserCanBulkAssociateEstabs2Groups) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("Bulk upload academies to trust<span class=\"visually-hidden\">&nbsp;</span>/<span class=\"visually-hidden\">&nbsp;</span>sponsor", "BulkAssociateEstabs2Groups"), Description = "Use this tool to upload new academies to the relevant trusts<span class=\"visually-hidden\">&nbsp;</span>/<span class=\"visually-hidden\">&nbsp;</span>sponsor" });
            if (UserCanBulkUpdateGovernors) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("Bulk update governance", "GovernorsBulkUpdate"), Description = "Update your governance data as a collection, rather than by individual entries." });
            return retVal;
        }

        public List<LinkAction> GetAdminActions(HtmlHelper htmlHelper)
        {
            var retVal = new List<LinkAction>();

            if (UserCanApprove) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("Approvals", "PendingApprovals"), Description = "View your pending changes and approve or reject." });
            if (UserCanManageAcademyOpenings) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("Manage academy openings", "ManageAcademyOpenings"), Description = "View details of proposed-to-open academies. Edit academy names and opening dates." });
            retVal.Add(new LinkAction { Link = htmlHelper.ActionLink("View data status", "ViewStatus", "DataQuality"), Description = "See when each team’s data was last updated. You can also confirm that your team's data is up to date." });
            retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("Change history", "ChangeHistoryCriteria"), Description = "View changes in the data relating to establishments and groups." });
            if (UserCanViewIndependentSchoolsSignificantDates) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("View independent schools' significant dates", "IndSchSearch"), Description = "View independent schools' &lsquo;Next general action required&rsquo; or &lsquo;Next action required by welfare&rsquo; dates." });
            if (UserCanDownloadMATClosureReport) retVal.Add(new LinkAction { Link = htmlHelper.RouteLink("View closed single academy trust and multi academy trust details", "DownloadMATClosureReport"), Description = "This is a list of single academy trusts and multi-academy trusts that are currently open on GIAS but reported closed by Companies House." });
            return retVal;
        }




    }
}
