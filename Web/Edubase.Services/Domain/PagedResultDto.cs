using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class PagedResultDto
    {
        public int Count { get; set; }
        public int Take { get; private set; }
        public int Skip { get; private set; }

        public int PageCount => (int) Math.Ceiling((double)Count / Take);
        public int CurrentPageIndex => (int) Math.Floor((double)Skip / Take) + 1;

        public PagedResultDto(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

    }
}
