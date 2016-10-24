using Edubase.Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Edubase.Common;

namespace Edubase.Web.UI.Models
{
    public class SearchCompaniesHouseModel
    {

        public string SearchText { get; set; }
        public PagedDto<CompanyProfileDto> Results { get; set; }
        public bool NotFound { get; set; }
        public bool IsNumberSearch => SearchText.IsInteger();
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}