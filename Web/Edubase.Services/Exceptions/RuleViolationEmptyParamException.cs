using System;

namespace Edubase.Services.Exceptions
{
    public class RuleViolationEmptyParamException : EdubaseException
    {
        public RuleViolationEmptyParamException() { }
        public RuleViolationEmptyParamException(string message) : base(message) { }
        public RuleViolationEmptyParamException(string message, Exception inner) : base(message, inner) { }
    }
}
