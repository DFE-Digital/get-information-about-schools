using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Enums
{
    public enum EstabNumberEntryPolicy
    {
        /// <summary>
        /// No establishment number is permitted
        /// </summary>
        NonePermitted,
        /// <summary>
        /// The user is free to enter any number 
        /// </summary>
        UserDefined,
        /// <summary>
        /// The system should generate the estab. number
        /// </summary>
        SystemGenerated
    }
}
