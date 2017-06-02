﻿using System;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Entity
{
    public class DataQualityStatus : TableEntity
    {
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
            PupilReferralUnits
        }

        public DataQualityStatus()
        {
            PartitionKey = "DataQuality";
        }

        public DataQualityEstablishmentType EstablishmentType
        {
            get
            {
                return (DataQualityEstablishmentType)int.Parse(RowKey);
            }
            set
            {
                RowKey = ((int)value).ToString();
            } 
        }

        public DateTime LastUpdated { get; set; }

        public string DataOwner { get; set; }

        public string Email { get; set; }
    }
}
