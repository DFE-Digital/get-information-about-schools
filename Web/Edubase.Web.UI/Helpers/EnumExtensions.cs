using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Runtime.Serialization;

namespace Edubase.Web.UI.Helpers
{
    public static class EnumExtensions
    {
        public static string GetEnumMember(this Enum en)
        {
            var type = en.GetType();
            var memberInfo = type.GetMember(en.ToString());
            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute), false);
                if (attributes.Length > 0)
                {
                    return ((EnumMemberAttribute) attributes[0]).Value;
                }
            }

            return en.ToString();
        }
    }
}