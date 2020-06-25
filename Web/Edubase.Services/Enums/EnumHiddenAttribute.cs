using System;
namespace Edubase.Services.Enums
{
    public class EnumGiasAttribute : Attribute
    {
        internal EnumGiasAttribute(bool hidden)
        {
            this.Hidden = hidden;
        }
        public bool Hidden { get; private set; }
    }
}
