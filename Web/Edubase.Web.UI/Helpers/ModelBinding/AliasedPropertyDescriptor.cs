using System;
using System.ComponentModel;

namespace Edubase.Web.UI.Helpers.ModelBinding
{
    internal sealed class AliasedPropertyDescriptor : PropertyDescriptor
    {
        public PropertyDescriptor Inner { get; private set; }

        public AliasedPropertyDescriptor(string alias, PropertyDescriptor inner)
          : base(alias, null)
        {
            Inner = inner;
        }

        public override bool CanResetValue(object component) => Inner.CanResetValue(component);
        public override Type ComponentType => Inner.ComponentType; 
        public override object GetValue(object component) => Inner.GetValue(component);
        public override bool IsReadOnly => Inner.IsReadOnly;
        public override Type PropertyType => Inner.PropertyType;
        public override void ResetValue(object component) => Inner.ResetValue(component);
        public override void SetValue(object component, object value) => Inner.SetValue(component, value);
        public override bool ShouldSerializeValue(object component) => Inner.ShouldSerializeValue(component);
    }
}