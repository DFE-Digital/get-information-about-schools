﻿using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Downloads;
using Edubase.Web.UI.Models.Search;
using System;

namespace Edubase.Web.UI.Areas.Establishments.Models.Search
{
    public class EstablishmentSearchDownloadViewModel : EstablishmentSearchViewModel, IDownloadGenerationProgressModel
    {
        public eFileFormat? FileFormat { get; set; }
        public eDataSet? Dataset { get; set; }
        public ProgressDto Progress { get { throw new Exception("Progress object not available"); } }
        public int Step => 2;
        public int TotalSteps => 4;
        public string DownloadName => "establishment";
        eFileFormat IDownloadGenerationProgressModel.FileFormat => FileFormat.Value;
        public bool IncludeEmailAddresses { get; set; }
        public bool IncludeIEBTFields { get; set; }
        public bool IncludeBringUpFields { get; set; }
        public bool IncludeChildrensCentreFields { get; set; }
        public bool IncludeLinks { get; set; }
        public bool AllowIncludeEmailAddresses { get; set; }
        public bool AllowIncludeIEBTFields { get; set; }
        public bool AllowIncludeBringUpFields { get; set; }
        public bool AllowIncludeChildrensCentreFields { get; set; }

        public bool AllowAnyExtraFields => AllowIncludeEmailAddresses || AllowIncludeIEBTFields || AllowIncludeBringUpFields || AllowIncludeChildrensCentreFields;
    }
}