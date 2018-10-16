using System;

namespace Edubase.Services.Exceptions
{
    [Serializable]
    public class RuleViolationEmptyParamException : EdubaseException
    {
        public RuleViolationEmptyParamException() { }
        public RuleViolationEmptyParamException(string message) : base(message) { }
        public RuleViolationEmptyParamException(string message, Exception inner) : base(message, inner) { }
        protected RuleViolationEmptyParamException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
