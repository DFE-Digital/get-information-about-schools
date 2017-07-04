using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Services.IntegrationEndPoints.CompaniesHouse.Models;
using System;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    public class SearchCompaniesHouseModel
    {

        public string SearchText { get; set; }
        public PagedDto<CompanyProfile> Results { get; set; }
        public bool NotFound { get; set; }
        public bool IsNumberSearch => SearchText.IsInteger();
        public int StartIndex { get; set; }
        public int PageSize { get; set; } = 50;
        
        public int Count => Results.Count;
        public int PageCount => (int)Math.Ceiling(Count / (double)PageSize);
    }
}