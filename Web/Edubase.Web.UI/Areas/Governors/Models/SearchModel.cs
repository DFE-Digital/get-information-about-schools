using FluentValidation;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Edubase.Common;
using Edubase.Data.Entity;
using static Edubase.Services.Core.FileDownloadFactoryService;
using Edubase.Services.Governors.Search;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    [Validator(typeof(SearchCriteriaModelValidator))]
    public class SearchModel
    {
        public string Forename { get; set; }
        public string Surname { get; set; }
        public int? RoleId { get; set; }
        public bool IncludeHistoric { get; set; }
        public string RoleName { get; set; }

        public long? Count { get; set; }
        public string Error { get; set; }
        public int PageCount { get; private set; }
        public int PageSize { get; set; }
        public int StartIndex { get; set; }
        public eFileFormat? FileFormat { get; set; }
        public AzureSearchResult<SearchGovernorDocument> Results { get; internal set; }
        public List<LookupItemViewModel> AppointingBodies { get; internal set; }
        public List<LookupItemViewModel> GovernorRoles { get; internal set; }

        public Dictionary<SearchGovernorDocument, string> EstablishmentNames { get; internal set; } = new Dictionary<SearchGovernorDocument, string>();
        public Dictionary<SearchGovernorDocument, string> GroupNames { get; internal set; } = new Dictionary<SearchGovernorDocument, string>();
        
        public void CalculatePageStats(int pageSize)
        {
            PageSize = pageSize;
            PageCount = (int)Math.Ceiling(Count.Value / (double)pageSize);
        }
    }

    public class SearchCriteriaModelValidator : AbstractValidator<SearchModel>
    {
        public SearchCriteriaModelValidator()
        {
            //RuleFor(x => x).Must(x => x.Forename.Clean() != null || x.RoleId.HasValue || x.Surname.Clean() != null).WithMessage("Please specify some criteria");
        }
    }
}