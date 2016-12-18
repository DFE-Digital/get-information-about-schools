using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class ServiceResultDto<T>
    {
        public T ReturnValue { get; set; }

        public bool Success => Status == eServiceResultStatus.Success;

        public bool IsPermissionDenied => Status == eServiceResultStatus.PermissionDenied;

        public bool NotFound => Status == eServiceResultStatus.NotFound;

        public eServiceResultStatus Status { get; set; }

        public ServiceResultDto()
        {

        }

        public ServiceResultDto(T value)
        {
            ReturnValue = value;
            Status = eServiceResultStatus.Success;
        }

        public ServiceResultDto(eServiceResultStatus status)
        {
            Status = status;

        }
    }

    public enum eServiceResultStatus
    {
        Success,
        PermissionDenied,
        NotFound
    }
}
