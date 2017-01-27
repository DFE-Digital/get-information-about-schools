using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Exceptions
{
    /// <summary>
    /// Represents an exception where the search query passed in is too complex/large for Azure Search.
    /// This is a domain exception because it's expected and the solution is for the user to 
    /// reduce the complexity of the query (i.e., reduce the number of selected filters)
    /// </summary>
    [Serializable]
    public class SearchQueryTooLargeException : EdubaseException
    {
        public SearchQueryTooLargeException() { }
        public SearchQueryTooLargeException(string message) : base(message) { }
        public SearchQueryTooLargeException(string message, Exception inner) : base(message, inner) { }
        protected SearchQueryTooLargeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
