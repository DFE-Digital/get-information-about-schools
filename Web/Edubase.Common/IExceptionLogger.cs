using System;

namespace Edubase.Common
{
    public interface IExceptionLogger
    {
        void Log(Exception ex);
    }
}
