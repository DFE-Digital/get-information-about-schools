using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Edubase.Web.UI.Helpers.ModelBinding
{
    internal class DefaultModelBinderEx : DefaultModelBinder
    {
        protected override PropertyDescriptorCollection GetModelProperties(ControllerContext controllerContext,
                            ModelBindingContext bindingContext)
        {
            var toReturn = base.GetModelProperties(controllerContext, bindingContext);
            var additional = new List<PropertyDescriptor>();

            foreach (var p in GetTypeDescriptor(controllerContext, bindingContext).GetProperties().Cast<PropertyDescriptor>())
            {
                foreach (var attr in p.Attributes.OfType<BindAliasAttribute>())
                {
                    additional.Add(new AliasedPropertyDescriptor(attr.Alias, p));

                    if (bindingContext.PropertyMetadata.ContainsKey(p.Name))
                        bindingContext.PropertyMetadata.Add(attr.Alias,
                              bindingContext.PropertyMetadata[p.Name]);
                }
            }

            return new PropertyDescriptorCollection(toReturn.Cast<PropertyDescriptor>().Concat(additional).ToArray());
        }

    }
}