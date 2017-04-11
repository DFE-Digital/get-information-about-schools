using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Security.ClaimsIdentityConverters
{
    public class SecureAccessClaimsIdConverter : IClaimsIdConverter
    {
        public ClaimsIdentity Convert(ClaimsIdentity id, string userId)
        {
            // Converts the ClaimsIdentity data sent by Secure Access into 
            // a new one supported by this application.
            // ... with JSON encoded payloads inside the claim value.
            

            throw new NotImplementedException();
        }
    }
}
