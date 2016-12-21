using Edubase.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Security.Permissions
{
    public abstract class Permission
    {
        public string ToClaimToken() => UriHelper.SerializeToUrlToken(this);
    }
}
