using Edubase.Services.Core;
using Edubase.Services.Downloads.Models;
using Edubase.Web.Resources;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class DownloadsViewModel
    {
        public class Section
        {
            public string Heading { get; set; }
            public string Paragraph { get; set; }
            public List<Section> SubSections { get; set; } = new List<Section>();
            public List<Tuple<string, FileDownload>> Files { get; set; } = new List<Tuple<string, FileDownload>>();
        }

        public FileDownload[] Downloads { get; set; }

        public PaginatedResult<ScheduledExtract> ScheduledExtracts { get; internal set; }

        public int ScheduledExtractsCount => (ScheduledExtracts?.Count).GetValueOrDefault();

        public bool AreScheduledExtractsAvailable => ScheduledExtractsCount > 0;

        public IEnumerable<Section> GetFileDownloadGroups()
        {
            var retVal = new List<Section>();

            var allEstabData = Downloads.Where(x => new[] { "all.edubase.data", "all.edubase.data.links" }.Contains(x.Tag));
            var openAcademiesAndFreeSchoolsData = Downloads.Where(x => new[] { "all.open.academies.and.free.schools", "all.open.academies.and.free.school.links" }.Contains(x.Tag));
            var openStateFundedSchoolsData = Downloads.Where(x => new[] { "all.open.state-funded.schools", "all.open.state-funded.school.links" }.Contains(x.Tag));
            var openChildrensCentresData = Downloads.Where(x => new[] { "all.open.childrens.centres", "all.open.childrens.centres.links" }.Contains(x.Tag));
            var openGroupData = Downloads.Where(x => new[] { "academy.sponsor.and.trust.links", "all.group.records", "all.group.links.records", "all.group.with.links.records", "academies.mat.membership" }.Contains(x.Tag));
            var allGovernorData = Downloads.Where(x => new[] { "all.governance.records", "all.mat.governance.records", "all.academy.governance.records", "all.la.maintained.governance.records" }.Contains(x.Tag));

            var miscData = Downloads.Where(x => !allEstabData.Concat(openAcademiesAndFreeSchoolsData)
                .Concat(openStateFundedSchoolsData)
                .Concat(openChildrensCentresData)
                .Concat(openGroupData)
                .Concat(allGovernorData)
                .Select(y => y.Tag)
                .Contains(x.Tag));


            if (allEstabData.Any() || openAcademiesAndFreeSchoolsData.Any() || openStateFundedSchoolsData.Any() || openChildrensCentresData.Any())
            {
                var section = new Section { Heading = "Establishments", Paragraph = "You can download the complete record for the speciﬁed establishment types. There's a separate file with links to any predecessor or successor establishments." };

                if (allEstabData.Any())
                {
                    section.SubSections.Add(new Section
                    {
                        Heading = "All establishment data",
                        Files = allEstabData.Select(x => new Tuple<string, FileDownload>(FileDownloadNames.ResourceManager.GetString(CleanTag(x.Tag)) ?? x.Name, x)).ToList()
                    });
                }

                if (openAcademiesAndFreeSchoolsData.Any())
                {
                    section.SubSections.Add(new Section
                    {
                        Heading = "Open academies and free schools data",
                        Files = openAcademiesAndFreeSchoolsData.Select(x => new Tuple<string, FileDownload>(FileDownloadNames.ResourceManager.GetString(CleanTag(x.Tag)) ?? x.Name, x)).ToList()
                    });
                }

                if (openStateFundedSchoolsData.Any())
                {
                    section.SubSections.Add(new Section
                    {
                        Heading = "Open state-funded schools data",
                        Files = openStateFundedSchoolsData.Select(x => new Tuple<string, FileDownload>(FileDownloadNames.ResourceManager.GetString(CleanTag(x.Tag)) ?? x.Name, x)).ToList()
                    });
                }

                if (openChildrensCentresData.Any())
                {
                    section.SubSections.Add(new Section
                    {
                        Heading = "Open children's centres data ",
                        Files = openChildrensCentresData.Select(x => new Tuple<string, FileDownload>(FileDownloadNames.ResourceManager.GetString(CleanTag(x.Tag)) ?? x.Name, x)).ToList()
                    });
                }

                retVal.Add(section);
            }

            if (openGroupData.Any())
            {
                var section = new Section
                {
                    Heading = "Establishment groups",
                    Paragraph = "You can download the complete record for the specified establishment group. There's a separate file with links to any establishments associated with the groups."
                };
                section.SubSections.Add(new Section
                {
                    Heading = "Open group data",
                    Files = openGroupData.Select(x => new Tuple<string, FileDownload>(FileDownloadNames.ResourceManager.GetString(CleanTag(x.Tag)) ?? x.Name, x)).ToList()
                });
                retVal.Add(section);
            }

            if (allGovernorData.Any())
            {
                var section = new Section { Heading = "Governors", Paragraph = "You can download the complete record for all governors listed within edubase." };
                section.SubSections.Add(new Section
                {
                    Heading = "All governor data",
                    Files = allGovernorData.Select(x => new Tuple<string, FileDownload>(FileDownloadNames.ResourceManager.GetString(CleanTag(x.Tag)) ?? x.Name, x)).ToList()
                });
                retVal.Add(section);
            }

            if (miscData.Any())
            {
                miscData.ForEach(x => x.AuthenticationRequired = true);

                var section = new Section { Heading = "Miscellaneous", Paragraph = "" };
                section.SubSections.Add(new Section
                {
                    Heading = "All data",
                    Files = miscData.Select(x => new Tuple<string, FileDownload>(FileDownloadNames.ResourceManager.GetString(CleanTag(x.Tag)) ?? x.Name, x)).ToList()
                });
                retVal.Add(section);
            }


            /*
             tag=all.edubase.data
             tag=all.edubase.data.links
             
             tag=all.open.academies.and.free.schools
             tag=all.open.academies.and.free.school.links
             
             tag=all.open.state-funded.schools
             tag=all.open.state-funded.school.links
             
             tag=academy.sponsor.and.trust.links
             tag=all.open.childrens.centres
             tag=all.governance.records
             tag=all.mat.governance.records
             tag=all.academy.governance.records
             tag=all.la.maintained.governance.records
             
             --
             tag=all.edubase.data
             tag=all.edubase.data.links
             tag=all.open.state-funded.schools
             tag=all.open.state-funded.school.links
             tag=all.open.academies.and.free.schools
             tag=all.open.academies.and.free.school.links
             tag=academy.sponsor.and.trust.links
             tag=all.open.childrens.centres
             +tag=all.open.childrens.centres.links
             tag=all.governance.records
             tag=all.mat.governance.records
             tag=all.academy.governance.records
             tag=all.la.maintained.governance.records
             +tag=all.group.records
             +tag=all.group.links.records
             +tag=all.group.with.links.records
             */


            return retVal;
        }

        private string CleanTag(string tag) => tag.Replace(".", "_").Replace("-", "_");
    }
}
