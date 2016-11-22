using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Exceptions
{
    public class PermissionDeniedException : EduSecurityException
    {
        public PermissionDeniedException()
        {
        }

        public PermissionDeniedException(string message) : base(message)
        {
        }

        public PermissionDeniedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PermissionDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
