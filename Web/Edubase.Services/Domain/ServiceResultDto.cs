using Edubase.Services.Exceptions;
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

        /// <summary>
        /// Gets the results, throwing an appropriate exception if the op was unsuccessful
        /// </summary>
        /// <returns></returns>
        public T GetResult()
        {
            if (Status == eServiceResultStatus.NotFound) throw new EntityNotFoundException();
            else if (Status == eServiceResultStatus.PermissionDenied) throw new PermissionDeniedException();
            return ReturnValue;
        }
    }

    public enum eServiceResultStatus
    {
        Success,
        PermissionDenied,
        NotFound
    }
}
