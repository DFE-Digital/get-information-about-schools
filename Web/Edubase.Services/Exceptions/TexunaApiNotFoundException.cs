using System;

namespace Edubase.Services.Exceptions
{

    [Serializable]
    public class TexunaApiNotFoundException : Exception
    {
        public TexunaApiNotFoundException() { }
        public TexunaApiNotFoundException(string message) : base(message) { }
        public TexunaApiNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected TexunaApiNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
