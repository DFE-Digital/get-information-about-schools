using System;
using System.Runtime.Serialization;
using Azure.Data.Tables;
using Azure;

namespace Edubase.Data.Entity;

public class DataQualityStatus : ITableEntity
{
    public string PartitionKey { get; set; } = "DataQuality";
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    /// <summary>
    /// Note that the enum indices are important to remain consistent:
    ///  - Azure Table Storage - table `DataQualityStatus` column `RowKey`
    /// </summary>
    public DataQualityEstablishmentType EstablishmentType
    {
        get => (DataQualityEstablishmentType) int.Parse(RowKey);
        set => RowKey = ((int) value).ToString();
    }

    public DateTime LastUpdated { get; set; }
    public string DataOwner { get; set; }
    public string Email { get; set; }

    public enum DataQualityEstablishmentType
    {
        [EnumMember(Value = "Academy openers")]
        AcademyOpeners,

        [EnumMember(Value = "Free school openers")]
        FreeSchoolOpeners,

        [EnumMember(Value = "Open academies and free schools")]
        OpenAcademiesAndFreeSchools,

        [EnumMember(Value = "LA maintained schools")]
        LaMaintainedSchools,

        [EnumMember(Value = "Independent schools")]
        IndependentSchools,

        [EnumMember(Value = "Pupil referral units")]
        PupilReferralUnits,

        [EnumMember(Value = "Secure academy 16-19 openers")]
        AcademySecure16to19Openers
    }
}
