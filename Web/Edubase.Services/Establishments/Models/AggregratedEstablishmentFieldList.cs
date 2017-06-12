using Edubase.Common.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Models
{
    public class AggregratedEstablishmentFieldList : EstablishmentFieldList
    {
        public AggregratedEstablishmentFieldList(EstablishmentFieldList list1, EstablishmentFieldList list2)
        {
            var props = ReflectionHelper.GetProperties(this, writeableOnly: true);
            foreach (var prop in props)
            {
                var value1 = ReflectionHelper.GetPropertyValue<bool>(list1, prop);
                var value2 = ReflectionHelper.GetPropertyValue<bool>(list2, prop);
                if(value1 && value2) ReflectionHelper.SetProperty(this, prop, true);
            }
        }
    }
}
