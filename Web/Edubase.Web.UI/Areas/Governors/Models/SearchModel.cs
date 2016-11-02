using FluentValidation;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Edubase.Common;
using Edubase.Data.Entity;

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

        public int Count { get; set; }
        public string Error { get; set; }
        public int PageCount { get; private set; }
        public int PageSize { get; set; }
        public int StartIndex { get; set; }
        public List<Governor> Results { get; set; }

        public void CalculatePageStats(int pageSize)
        {
            PageSize = pageSize;
            PageCount = (int)Math.Ceiling(Count / (double)pageSize);
        }
    }

    public class SearchCriteriaModelValidator : AbstractValidator<SearchModel>
    {
        public SearchCriteriaModelValidator()
        {
            RuleFor(x => x).Must(x => x.Forename.Clean() != null || x.RoleId.HasValue || x.Surname.Clean() != null).WithMessage("Please specify some criteria");
        }
    }
}