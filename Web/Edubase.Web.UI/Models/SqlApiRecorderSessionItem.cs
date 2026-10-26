using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Web.UI.Models
{
    [Table("ApiRecorderSessionItems", Schema = "FrontEnd")]
    public class SqlApiRecorderSessionItem
    {
        public SqlApiRecorderSessionItem() { }

        [Key, Column(Order = 0)]
        public string PartitionKey { get; set; }
        [Key, Column(Order = 1)]
        public string RowKey { get; set; }
        public string HttpMethod { get; set; }
        public string Path { get; set; }
        public string RequestHeaders { get; set; }
        public string ResponseHeaders { get; set; }
        public string RawRequestBody { get; set; }
        public string RawResponseBody { get; set; }
        public string ElapsedTimeSpan { get; set; }
        public double ElapsedMS { get; set; }
    }
}
