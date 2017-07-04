using Edubase.Services.Domain;

namespace Edubase.Web.UI.Models
{
    public class LookupItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public LookupItemViewModel()
        {

        }
        
        public LookupItemViewModel(LookupDto item)
        {
            Id = item.Id;
            Name = item.Name;
        }

        public LookupItemViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }

        
        public override string ToString() => Name;
    }
}