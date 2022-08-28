using System.Collections.Generic;

namespace Edubase.Web.UI.Models.Guidance
{
    public class GuidanceLaNameCodeViewModel
    {
        public string DownloadType { get; set; }
        public List<LaNameCodes> EnglishLas { get; set; }
        public List<LaNameCodes> WelshLas { get; set; }
        public List<LaNameCodes> OtherLas { get; set; }
    }
}
