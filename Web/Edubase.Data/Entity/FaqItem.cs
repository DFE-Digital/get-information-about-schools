using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.ComponentModel.DataAnnotations;

namespace Edubase.Data.Entity
{
    public enum FaqSection
    {
        [Display(Name = "", Description = "General FAQ")]
        General = 0,
        [Display(Name = "Personal data and security", Description = "Personal data and security")]
        PersonalData = 10
    }

    public class FaqItem : TableEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int DisplayOrder { get; set; }
        public int SectionId { get; set; }

        [IgnoreProperty]
        public FaqSection Section
        {
            get => (FaqSection) SectionId;
            set => SectionId = (int) value;
        }

        public FaqItem()
        {
            PartitionKey = string.Empty;
            RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
        }
    }
}
