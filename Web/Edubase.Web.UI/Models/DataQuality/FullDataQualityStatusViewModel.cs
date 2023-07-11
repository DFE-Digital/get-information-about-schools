using System.Collections.Generic;

namespace Edubase.Web.UI.Models.DataQuality
{
    public class FullDataQualityStatusViewModel
    {
        public List<FullDataQualityStatusItem> Items { get; set; }
        public bool DataUpdated { get; set; }
        public bool UserCanUpdateLastUpdated { get; set; }
        public bool UserCanUpdateDataOwnerDetails { get; set; }
    }
}
