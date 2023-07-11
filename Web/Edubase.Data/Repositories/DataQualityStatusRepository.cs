using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Edubase.Common;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;

namespace Edubase.Data.Repositories
{
    public class DataQualityStatusRepository : TableStorageBase<DataQualityStatus>, IDataQualityStatusRepository
    {
        public DataQualityStatusRepository()
            : base("DataConnectionString")
        {
            SeedTable();
        }

        public async Task<List<DataQualityStatus>> GetAllAsync()
        {
            var query = Table.CreateQuery<DataQualityStatus>();
            var results = await query.ExecuteSegmentedAsync(null);
            return results.Results;
        }


        public async Task UpdateDataQualityAsync(DataQualityStatus.DataQualityEstablishmentType establishmentType, DateTime lastUpdated)
        {
            var query = Table.CreateQuery<DataQualityStatus>().Where(d => d.RowKey == ((int)establishmentType).ToString()).AsTableQuery();
            var results = await query.ExecuteSegmentedAsync(null);
            var dataQualityRecord = results.FirstOrDefault();
            if (dataQualityRecord != null)
            {
                dataQualityRecord.LastUpdated = lastUpdated;

                var operation = TableOperation.InsertOrReplace(dataQualityRecord);
                await Table.ExecuteAsync(operation);
            }
        }

        public async Task UpdateDataQualityDataOwnerDetailsAsync(DataQualityStatus.DataQualityEstablishmentType establishmentType, string dataOwnerName, string dataOwnerEmail)
        {
            var query = Table.CreateQuery<DataQualityStatus>().Where(d => d.RowKey == ((int)establishmentType).ToString()).AsTableQuery();
            var results = await query.ExecuteSegmentedAsync(null);
            var dataQualityRecord = results.FirstOrDefault();
            if (dataQualityRecord != null)
            {
                dataQualityRecord.DataOwner = dataOwnerName;
                dataQualityRecord.Email = dataOwnerEmail;

                var operation = TableOperation.InsertOrReplace(dataQualityRecord);
                await Table.ExecuteAsync(operation);
            }
        }

        private void SeedTable()
        {
            var seedData = new List<DataQualityStatus>
            {
                new DataQualityStatus
                {
                    EstablishmentType = DataQualityStatus.DataQualityEstablishmentType.AcademyOpeners,
                    LastUpdated = new DateTime(1601, 1, 1),
                    DataOwner = "Academies Operation and Strategy",
                    Email = "academies.data@education.gov.uk"
                },
                new DataQualityStatus
                {
                    EstablishmentType = DataQualityStatus.DataQualityEstablishmentType.FreeSchoolOpeners,
                    LastUpdated = new DateTime(1601, 1, 1),
                    DataOwner = "Free Schools Delivery 1",
                    Email = "freeschools.pre-opening@education.gov.uk"
                },
                new DataQualityStatus
                {
                    EstablishmentType = DataQualityStatus.DataQualityEstablishmentType.OpenAcademiesAndFreeSchools,
                    LastUpdated = new DateTime(1601, 1, 1),
                    DataOwner = "EFA Academies Enquiries Team",
                    Email = "academiesdata.esfa@education.gov.uk"
                },
                new DataQualityStatus
                {
                    EstablishmentType = DataQualityStatus.DataQualityEstablishmentType.LaMaintainedSchools,
                    LastUpdated = new DateTime(1601, 1, 1),
                    DataOwner = "School Organisation Team",
                    Email = "schoolorganisation.notifications@education.gov.uk"
                },
                new DataQualityStatus
                {
                    EstablishmentType = DataQualityStatus.DataQualityEstablishmentType.IndependentSchools,
                    LastUpdated = new DateTime(1601, 1, 1),
                    DataOwner = "Independent Education and Boarding Team",
                    Email = "registration.enquiries@education.gov.uk"
                },
                new DataQualityStatus
                {
                    EstablishmentType = DataQualityStatus.DataQualityEstablishmentType.PupilReferralUnits,
                    LastUpdated = new DateTime(1601, 1, 1),
                    DataOwner = "Alternative Provision and Exclusions Team",
                    Email = "alternativeprovision.pru@education.gov.uk"
                },
            };

            var query = Table.CreateQuery<DataQualityStatus>();
            var results = query.Execute();
            if (!results.Any())
            {
                foreach (var data in seedData)
                {
                    var operation = TableOperation.Insert(data);
                    Table.Execute(operation);
                }
            }
        }
    }
}
