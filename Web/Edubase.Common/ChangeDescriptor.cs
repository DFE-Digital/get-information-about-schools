namespace Edubase.Common
{
    public struct ChangeDescriptor
    {
        public string Name { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }
        
        public ChangeDescriptor(string name, object newValue, object oldValue)
        {
            Name = name;
            NewValue = newValue?.ToString();
            OldValue = oldValue?.ToString();
        }

        public override string ToString() => 
            $"Name = {Name}; Old Value = {OldValue ?? "<empty>"}, New Value = {NewValue ?? "<empty>"}";
    }
}
