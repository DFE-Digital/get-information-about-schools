using System;

namespace Edubase.Web.UI.Helpers.ModelBinding
{
    /// <summary>
    /// Allows aliases to be used for model properties during request binding.
    /// The ASP.NET Core binding pipeline resolves these aliases via custom
    /// value-provider infrastructure.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    internal class BindAliasAttribute : Attribute
    {
        public BindAliasAttribute(string alias)
        {
            Alias = alias;
        }

        public string Alias { get; private set; }
    }
}
