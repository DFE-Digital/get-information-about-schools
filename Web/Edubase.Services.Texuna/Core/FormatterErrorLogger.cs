using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;

namespace Edubase.Services.Texuna.Core
{
    public class FormatterErrorLogger : IFormatterLogger
    {
        public List<FormatterError> Errors { get; private set; } = new List<FormatterError>();

        public void LogError(string errorPath, string errorMessage) => Errors.Add(new FormatterError { ErrorPath = errorPath, ErrorMessage = errorMessage });

        public void LogError(string errorPath, Exception exception) => Errors.Add(new FormatterError { ErrorPath = errorPath, Exception = exception, ErrorMessage = exception.Message });
    }
}
