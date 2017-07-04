using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.Google.Models
{
    internal class StructuredFormatting
    {
        public string main_text { get; set; }
        public List<MatchedSubstring> main_text_matched_substrings { get; set; }
        public string secondary_text { get; set; }

    }
}
