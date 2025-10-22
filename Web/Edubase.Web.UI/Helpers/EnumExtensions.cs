using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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


        public static string EnumDisplayNameFor(this Enum en)
        {
            var type = en.GetType();
            var member = type.GetMember(en.ToString());
            var displayName = en.ToString();

            var attributes = member.Select(e => e.GetCustomAttributes(typeof(DisplayAttribute), false)).FirstOrDefault();
            if (attributes != null && attributes.Length > 0)
            {
                displayName = ((DisplayAttribute) attributes[0]).Name;
            }

            return displayName;
        }
        public static string EnumDisplayDescriptionFor(this Enum en)
        {
            var type = en.GetType();
            var member = type.GetMember(en.ToString());
            var displayDescription = en.ToString();

            var attributes = member.Select(e => e.GetCustomAttributes(typeof(DisplayAttribute), false)).FirstOrDefault();
            if (attributes != null && attributes.Length > 0)
            {
                displayDescription = ((DisplayAttribute) attributes[0]).Description;
            }

            return displayDescription;
        }

        public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T) attributes[0] : null;
        }
    }
}
