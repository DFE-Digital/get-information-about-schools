using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Downloads;
using Edubase.Web.UI.Helpers.ModelBinding;
using Edubase.Web.UI.Models.Search;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Edubase.Web.UI.Areas.Establishments.Models.Search
{
    public class EstablishmentSearchDownloadViewModel : EstablishmentSearchViewModel, IDownloadGenerationProgressModel
    {
        public const string SelectedCustomFieldsBindAlias = "zm";

        public class CustomFieldGroup
        {
            public string Category { get; set; }
            public IEnumerable<EstablishmentSearchDownloadCustomField> CustomFields { get; set; }
        }

        public eFileFormat? FileFormat { get; set; }
        public eDataSet? Dataset { get; set; }
        public ProgressDto Progress { get { throw new Exception("Progress object not available"); } }
        public int? Step => null;
        public int? TotalSteps => null;
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

        public IList<EstablishmentSearchDownloadCustomField> CustomFields { get; internal set; }

        [BindAlias(SelectedCustomFieldsBindAlias)]
        public string[] SelectedCustomFields { get; set; } = new string[0];

        public IEnumerable<CustomFieldGroup> CustomFieldsByCategory => CustomFields.GroupBy(x => x.Category).OrderBy(x => x.Key).Select(x => new CustomFieldGroup { Category = x.FirstOrDefault()?.Category, CustomFields = x.OrderBy(y => y.Name).AsEnumerable() });

        public bool ViewedCustomFields { get; set; } = false;
    }
}
