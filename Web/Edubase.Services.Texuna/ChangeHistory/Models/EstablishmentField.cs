using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Texuna.ChangeHistory.Models
{
    public class EstablishmentField
    {
        public string Key { get; set; }

        public string Text { get; set; }

        public EstablishmentField()
        {
                
        }

        public EstablishmentField(string key, string text)
        {
            Key = key;
            Text = text;
        }
    }
}
