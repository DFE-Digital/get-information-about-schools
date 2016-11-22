using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Security.ClaimsIdentityConverters
{
    public interface IClaimsIdConverter
    {
        ClaimsIdentity Convert(ClaimsIdentity id);
    }
}
