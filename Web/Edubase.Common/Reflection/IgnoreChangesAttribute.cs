using System;

namespace Edubase.Common.Reflection
{
    /// <summary>
    /// If specified on a property, then the ReflectionHelper.DetectChanges will ignore changes to this property.
    /// </summary>

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class IgnoreChangesAttribute : Attribute
    {
        
    }
}
