using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Edubase.Services.Establishments.Models
{
    public class BulkUpdateDto
    {
        public const string EduBaseBulkUpload = "Bulk Upload";
        public const string MultipleColumnsFile = "Multiple Columns file";

        public enum eBulkUpdateType
        {
            [Display(Name = "Bulk Upload")]
            EduBaseBulkUpload,

            [Display(Name = "Multiple Columns File")]
            MultipleColumnsFile
        }

        [JsonIgnore]
        public string FileName { get; set; }

        public DateTime? EffectiveDate { get; set; }

        [JsonIgnore]
        public bool OverrideCRProcess { get; set; }

        [JsonIgnore]
        public eBulkUpdateType BulkFileType { get; set; }

        [JsonProperty("type")]
        public string BulkFileTypeDescription => BulkFileType == eBulkUpdateType.EduBaseBulkUpload ? "EduBase Bulk Upload" : "Multiple Columns File";
    }
}
