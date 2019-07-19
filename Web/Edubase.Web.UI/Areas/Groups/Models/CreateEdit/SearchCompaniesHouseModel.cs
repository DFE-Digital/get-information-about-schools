using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Services.IntegrationEndPoints.CompaniesHouse.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    public class SearchCompaniesHouseModel
    {
        public bool Search { get; set; } = false;

        public string SearchText { get; set; }
        public PagedDto<CompanyProfile> Results { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; } = 50;
        
        public int Count => Results.Count;
        public int PageCount => (int)Math.Ceiling(Count / (double)PageSize);
    }
}
