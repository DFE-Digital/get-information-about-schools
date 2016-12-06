using Edubase.Services.Establishments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Search
{
    public class EstablishmentSearchPayload
    {
        public EstablishmentSearchPayload()
        {

        }

        public EstablishmentSearchPayload(string orderBy, int skip, int take)
        {
            OrderBy = new List<string> { nameof(SearchEstablishmentDocument.Name) };
            Skip = skip;
            Take = take;
        }
        public string Text { get; set; }
        public EstablishmentSearchFilters Filters { get; set; } = new EstablishmentSearchFilters();
        public int Skip { get; set; }
        public int Take { get; set; } = 10;

        public static readonly IList<string> FullTextSearchFields = new[] { nameof(EstablishmentModel.Name) }.ToList();
        public IList<string> OrderBy { get; set; }
    }
}
