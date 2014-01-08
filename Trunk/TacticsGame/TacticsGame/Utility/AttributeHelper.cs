using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using TacticsGame.Attributes;

namespace TacticsGame
{
    public static class AttributeHelper
    {
        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        public static T GetAttributeOfType<T>(this IConvertible enumVal) where T : System.Attribute
        {
            try
            {
                Type type = enumVal.GetType();
                MemberInfo[] memInfo = type.GetMember(enumVal.ToString());
                object[] attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
                return (T)attributes[0];
            }
            catch
            {
                return null;
            }
        }

        public static string GetDisplayName(this IConvertible enumVal)
        {
            DisplayNameAttribute attr = GetAttributeOfType<DisplayNameAttribute>(enumVal);
            if (attr == null)
            {
                return enumVal.ToString();
            }

            return attr.DisplayName;
        }
    }
}
