using System;
using System.Collections.Generic;

namespace Web.Services.Search
{
    public class SearchException : Exception
    {
        private static readonly Dictionary<SearchExceptionTypes, string> MessagesByType = new Dictionary<SearchExceptionTypes, string>
        {
            { SearchExceptionTypes.InvalidUrn, "The URN you entered does not match to any school. Check the number and try again." },
            { SearchExceptionTypes.InvalidLaEstab, "The LAESTAB you entered does not match to any school. Check the number and try again." }
        };

        public SearchException(SearchExceptionTypes type)
            : base(MessagesByType[type])
        {
        }
    }
}