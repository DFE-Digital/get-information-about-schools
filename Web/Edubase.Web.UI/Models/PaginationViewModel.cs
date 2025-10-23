using System;

namespace Edubase.Web.UI.Models
{
    public class PaginationViewModel
    {
        public struct Result
        {
            public int Count { get; set; }
        }

        public int StartIndex { get; private set; }
        public int PageSize { get; private set; }
        public int Count { get; private set; }
        public Result Results => new Result { Count = Count };
        public int PageCount => (int)Math.Ceiling(Count / (double)PageSize);

        public PaginationViewModel(int skip, int take, int total)
        {
            StartIndex = skip / take;
            PageSize = take;
            Count = total;
        }
    }
}
