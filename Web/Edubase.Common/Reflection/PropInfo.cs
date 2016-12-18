using System;

namespace Edubase.Common.Reflection
{
    public struct PropInfo
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public object Value { get; set; }
    }
}
