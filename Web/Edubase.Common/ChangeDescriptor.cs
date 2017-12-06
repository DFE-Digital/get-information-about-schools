using System;

namespace Edubase.Common
{
    public class ChangeDescriptor
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }

        public ChangeDescriptor(string name, object newValue, object oldValue)
        {
            Name = name;
            NewValue = ToString(newValue);
            OldValue = ToString(oldValue);
        }

        public ChangeDescriptor(string name, string displayName, object newValue, object oldValue)
            : this(name, newValue, oldValue)
        {
            DisplayName = displayName;
        }

        private string ToString(object val) => val != null && val is DateTime ? ((DateTime)val).ToString("dd/MM/yyyy") : val?.ToString();

        private string ToString(object val) => val != null && val is DateTime ? ((DateTime)val).ToString("dd/MM/yyyy") : val?.ToString();

        public override string ToString() =>
            $"Name = {Name}; Old Value = {OldValue ?? "<empty>"}, New Value = {NewValue ?? "<empty>"}";
    }
}
