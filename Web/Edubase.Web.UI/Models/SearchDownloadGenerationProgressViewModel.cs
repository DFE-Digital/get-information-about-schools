using Edubase.Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Edubase.Web.UI.Models.AdvancedSearchViewModel;

namespace Edubase.Web.UI.Models
{
    public class SearchDownloadGenerationProgressViewModel
    {
        public SearchDownloadGenerationProgressDto Progress { get; set; }

        public eSearchCollection SearchCollection { get; set; }

        public int GetTotalSteps() => SearchCollection == eSearchCollection.Establishments ? 4 : 3;

        public string GetDownloadName()
        {
            if (SearchCollection == eSearchCollection.Establishments) return "establishment";
            else if (SearchCollection == eSearchCollection.Groups) return "establishment group";
            else return "governor";
        }

        public SearchDownloadGenerationProgressViewModel(SearchDownloadGenerationProgressDto progressDto, eSearchCollection searchCollection)
        {
            Progress = progressDto;
            SearchCollection = searchCollection;
        }
    }
}