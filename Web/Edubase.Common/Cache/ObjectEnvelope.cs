using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Common.Cache
{
    /// <summary>
    /// Wraps an object in an envelope together with any additional properties
    /// ready for serialisation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectEnvelope<T>
    {
        public T Object { get; set; }
        public Dictionary<string, object> Properties { get; set; }

        public ObjectEnvelope(T obj, Dictionary<string, object> properties)
        {
            Object = obj;
            Properties = properties;
        }

        public ObjectEnvelope()
        {

        }
    }
}
