using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Data.Entity.Permissions
{
    public enum ActionType
    {
        Update,
        Approve
    }

    /// <summary>
    /// Flags a column as requiring an approval process.
    /// Used on its own, it means the field can be changed by any role, but still requires approval.
    /// </summary>
    public class RequiresApprovalAttribute : Attribute { }

    /// <summary>
    /// Prevents restrictive roles from updating or approving changes on this property
    /// Also, flags a column as requiring an approval process
    /// </summary>
    public class RestrictPermissionAttribute : RequiresApprovalAttribute { }

    /// <summary>
    /// Restricts a specific role from performing a specific action
    /// Also, flags a column as requiring an approval process
    /// </summary>
    public class RestrictActionAttribute : RequiresApprovalAttribute
    {
        public ActionType ActionType { get; set; }
        public string Role { get; set; }
        public RestrictActionAttribute(string role, ActionType actionType)
        {
            Role = role;
            ActionType = actionType;
        }
    }

    public static class PermissionUtil
    {
        public static bool AllowUpdate(string role, RequiresApprovalAttribute attribute)
        {
            if (attribute is RestrictActionAttribute)
            {
                var attr = attribute as RestrictActionAttribute;
                return !(attr.ActionType == ActionType.Update && attr.Role == role);
            }
            else if (attribute is RestrictPermissionAttribute) return false;
            else return true;
        }

        public static bool AllowApproval(string role, RequiresApprovalAttribute attribute)
        {
            if (attribute is RestrictActionAttribute)
            {
                var attr = attribute as RestrictActionAttribute;
                return !(attr.ActionType == ActionType.Approve && attr.Role == role);
            }
            else if (attribute is RestrictPermissionAttribute) return false;
            else return true;
        }

        public class GetEntityColumns_Item
        {
            public string Name { get; set; }
            public RequiresApprovalAttribute Attribute { get; set; }
        }

        /// <summary>
        /// Retrieves an array of underlying column names for an entity, including complex types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dc"></param>
        /// <returns></returns>
        public static GetEntityColumns_Item[] GetRestrictiveColumns<T>(ApplicationDbContext dc)
        {
            var workspace = ((IObjectContextAdapter)dc).ObjectContext.MetadataWorkspace;
            var entityType = typeof(T);
            var typeName = entityType.Name;
            var entityMetaData = (from meta in workspace.GetItems(DataSpace.CSpace)
                                 .Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType
                                    && (m as EntityType).Name == typeName)
                                  select meta).First() as EntityType;

            var props = entityMetaData.Properties
                .SelectMany(x =>
                {
                    if (!x.IsComplexType)
                    {
                        var prop = entityType.GetProperty(x.Name);
                        return new[]
                        {
                            new GetEntityColumns_Item
                            {
                                Name = prop.Name,
                                Attribute = prop.GetCustomAttributes(typeof(RequiresApprovalAttribute), false).Cast<RequiresApprovalAttribute>().FirstOrDefault()
                            }
                        };
                    }
                    else
                    {
                        var type = Type.GetType(x.ComplexType.FullName.Replace("Edubase.Data.Entity", "Edubase.Data.Entity.ComplexTypes"));
                        return x.ComplexType.Properties.Select(c =>
                        {
                            var prop = type.GetProperty(c.Name);
                            return new GetEntityColumns_Item
                            {
                                Name = string.Concat(x.Name, "_", c.Name),
                                Attribute = prop.GetCustomAttributes(typeof(RequiresApprovalAttribute), false).Cast<RequiresApprovalAttribute>().FirstOrDefault()

                            };
                        });
                    }
                });

            return props.Where(x => x.Attribute != null).ToArray();
        }
    }
}
