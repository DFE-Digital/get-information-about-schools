using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class SecurityDto<T>
    {
        public T Data { get; set; }

        public bool CanUserEdit { get; set; }
    }
}
