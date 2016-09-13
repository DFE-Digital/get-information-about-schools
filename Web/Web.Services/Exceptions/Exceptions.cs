using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Services.Exceptions
{

    [Serializable]
    public class LaNameNotFoundException : Exception
    {
        public LaNameNotFoundException() { }
        public LaNameNotFoundException(string message) : base(message) { }
        public LaNameNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected LaNameNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
