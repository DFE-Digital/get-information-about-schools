using System;

namespace Edubase.Services.Exceptions
{

    public class TexunaApiNotFoundException : Exception
    {
        public TexunaApiNotFoundException() { }
        public TexunaApiNotFoundException(string message) : base(message) { }
        public TexunaApiNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
