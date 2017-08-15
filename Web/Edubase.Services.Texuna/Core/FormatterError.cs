using System;

namespace Edubase.Services.Texuna.Core
{
    public class FormatterError
    {
        public string ErrorPath { get; set; }
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }
    }
}
