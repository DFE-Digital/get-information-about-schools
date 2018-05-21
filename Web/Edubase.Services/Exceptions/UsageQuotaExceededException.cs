using System;

namespace Edubase.Services.Exceptions
{
    [Serializable]
    public class UsageQuotaExceededException : EdubaseException
    {
        public UsageQuotaExceededException() : base("Your usage quota has been exceeded. Please try again in a minute or two.") { }
    }
}
