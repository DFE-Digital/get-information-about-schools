using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Edubase.Common;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;

namespace Edubase.Data.Repositories;

public class DataQualityStatusRepository : TableStorageBase<DataQualityStatus>, IDataQualityStatusRepository
{

    private const string DataQualityStatusPartitionKey = "DataQuality";

    public DataQualityStatusRepository()
        : base("DataConnectionString")
    {
        SeedTable();
    }

    public async Task<IEnumerable<DataQualityStatus>> GetAllAsync()
    {
        List<DataQualityStatus> results = [];

        await foreach (var entity in Table.QueryAsync<DataQualityStatus>())
        {
            results.Add(entity);
        }

        return results;
    }


    public async Task UpdateDataQualityAsync(DataQualityStatus.DataQualityEstablishmentType establishmentType, DateTime lastUpdated)
    {
        var response = await Table.GetEntityIfExistsAsync<DataQualityStatus>(
            partitionKey: DataQualityStatusPartitionKey,
            rowKey: ((int) establishmentType).ToString());

        if (response.HasValue)
        {
            var entity = response.Value;
            entity.LastUpdated = lastUpdated;
            await Table.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace);
        }

    }


    public async Task UpdateDataQualityDataOwnerDetailsAsync(DataQualityStatus.DataQualityEstablishmentType establishmentType, string dataOwnerName, string dataOwnerEmail)
    {
        var response = await Table.GetEntityIfExistsAsync<DataQualityStatus>(
            partitionKey: DataQualityStatusPartitionKey,
            rowKey: ((int) establishmentType).ToString());

        if (response.HasValue)
        {
            var entity = response.Value;
            entity.DataOwner = dataOwnerName;
            entity.Email = dataOwnerEmail;
            await Table.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace);
        }
    }

    private void SeedTable()
    {
        var seedData = new List<DataQualityStatus>
        {
            new()
            {
                EstablishmentType = DataQualityStatus.DataQualityEstablishmentType.AcademyOpeners,
                LastUpdated = new DateTime(1601, 1, 1),
                DataOwner = "Academies Operation and Strategy",
                Email = "academies.data@education.gov.uk"
            },
            new()
            {
                EstablishmentType = DataQualityStatus.DataQualityEstablishmentType.FreeSchoolOpeners,
                LastUpdated = new DateTime(1601, 1, 1),
                DataOwner = "Free Schools Delivery 1",
                Email = "freeschools.pre-opening@education.gov.uk"
            },
            new()
            {
                EstablishmentType = DataQualityStatus.DataQualityEstablishmentType.OpenAcademiesAndFreeSchools,
                LastUpdated = new DateTime(1601, 1, 1),
                DataOwner = "EFA Academies Enquiries Team",
                Email = "academiesdata.esfa@education.gov.uk"
            },
            new()
            {
                EstablishmentType = DataQualityStatus.DataQualityEstablishmentType.LaMaintainedSchools,
                LastUpdated = new DateTime(1601, 1, 1),
                DataOwner = "School Organisation Team",
                Email = "schoolorganisation.notifications@education.gov.uk"
            },
            new()
            {
                EstablishmentType = DataQualityStatus.DataQualityEstablishmentType.IndependentSchools,
                LastUpdated = new DateTime(1601, 1, 1),
                DataOwner = "Independent Education and Boarding Team",
                Email = "registration.enquiries@education.gov.uk"
            },
            new()
            {
                EstablishmentType = DataQualityStatus.DataQualityEstablishmentType.PupilReferralUnits,
                LastUpdated = new DateTime(1601, 1, 1),
                DataOwner = "Alternative Provision and Exclusions Team",
                Email = "alternativeprovision.pru@education.gov.uk"
            },
        };


        foreach (var _ in Table.Query<DataQualityStatus>())
        {
            // Found at least one entity — table is not empty
            return;
        }

        // Table is empty — seed it
        foreach (var data in seedData)
        {
            Table.AddEntity(data);
        }

    }
}
