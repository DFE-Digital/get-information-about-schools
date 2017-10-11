using Edubase.Services.Domain;

namespace Edubase.Web.UI.Models
{
    public class LookupItemViewModel : AbstractLookupItemViewModel<int>
    {
        public LookupItemViewModel()
        {
        }

        public LookupItemViewModel(int id, string name)  : base(id, name)
        {
        }

        public LookupItemViewModel(LookupDto item)
        {
            Id = item.Id;
            Name = item.Name;
        }
    }

    public class StringLookupItemViewModel : AbstractLookupItemViewModel<string>
    {
        public StringLookupItemViewModel()
        {
        }

        public StringLookupItemViewModel(string id, string name) : base(id, name)
        {
        }

        public StringLookupItemViewModel(LookupDto item)
        {
            Id = item.Id.ToString();
            Name = item.Name;
        }
    }

    public abstract class AbstractLookupItemViewModel<T>
    {
        protected AbstractLookupItemViewModel()
        {
        }

        protected AbstractLookupItemViewModel(T id, string name)
        {
            Id = id;
            Name = name;
        }

        public T Id { get; set; }
        public string Name { get; set; }

        public override string ToString() => Name;
    }
}