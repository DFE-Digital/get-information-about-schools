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
            NewValue = newValue?.ToString();
            OldValue = oldValue?.ToString();
        }

        public ChangeDescriptor(string name, string displayName, object newValue, object oldValue)
            : this(name, newValue, oldValue)
        {
            DisplayName = displayName;
        }

        public override string ToString() => 
            $"Name = {Name}; Old Value = {OldValue ?? "<empty>"}, New Value = {NewValue ?? "<empty>"}";
    }
}
