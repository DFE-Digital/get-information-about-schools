using Edubase.Services.Downloads.Models;
using Edubase.Web.Resources;
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

        public ScheduledExtractsResult ScheduledExtracts { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public int ScheduledExtractsCount => (ScheduledExtracts?.Count).GetValueOrDefault();

        public bool AreScheduledExtractsAvailable => ScheduledExtractsCount > 0;

        public PaginationViewModel Pagination => new PaginationViewModel(Skip, Take, ScheduledExtractsCount);


        public IEnumerable<Section> GetFileDownloadGroups()
        {
            var retVal = new List<Section>();

            var allEstabData = Downloads.Where(x => new[] { "all.edubase.data", "all.edubase.data.links" }.Contains(x.Tag));
            var openAcademiesAndFreeSchoolsData = Downloads.Where(x => new[] { "all.open.academies.and.free.schools", "all.open.academies.and.free.school.links" }.Contains(x.Tag));
            var openStateFundedSchoolsData = Downloads.Where(x => new[] { "all.open.state-funded.schools", "all.open.state-funded.school.links" }.Contains(x.Tag));
            var openChildrensCentresData = Downloads.Where(x => new[] { "all.open.childrens.centres" }.Contains(x.Tag));
            var openGroupData = Downloads.Where(x => new[] { "academy.sponsor.and.trust.links" }.Contains(x.Tag));
            var allGovernorData = Downloads.Where(x => new[] { "all.governance.records", "all.mat.governance.records", "all.academy.governance.records", "all.la.maintained.governance.records" }.Contains(x.Tag));
            
            
            if(allEstabData.Any()|| openAcademiesAndFreeSchoolsData.Any() || openStateFundedSchoolsData.Any() || openChildrensCentresData.Any())
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
                var section = new Section { Heading = "Establishment groups", Paragraph = "You can download the complete record for the specified establishment group. There's a separate file with links to any predecessor or successor establishments." };
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
             */


            return retVal;
        }

        private string CleanTag(string tag) => tag.Replace(".", "_").Replace("-", "_");
    }
}