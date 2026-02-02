using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories;

public class DataQualityStatusRepository : IDataQualityStatusRepository
{
    private const string DataQualityStatusPartitionKey = "DataQuality";
    private const string TableNameKey = "DataQualityStatus";

    private readonly TableClient _dataQualityStatusTableClient;

    public DataQualityStatusRepository(TableServiceClient tableServiceClient)
    {
        _dataQualityStatusTableClient = tableServiceClient.GetTableClient(TableNameKey);
    }

    public async Task EnsureSeededAsync()
    {
        await foreach (var _ in _dataQualityStatusTableClient.QueryAsync<DataQualityStatus>())
        {
            return; // Table is not empty
        }

        foreach (var data in GetSeedData())
        {
            data.PartitionKey = DataQualityStatusPartitionKey;
            data.RowKey = ((int) data.EstablishmentType).ToString();
            await _dataQualityStatusTableClient.AddEntityAsync(data);
        }
    }

    public async Task<IEnumerable<DataQualityStatus>> GetAllAsync()
    {
        var results = new List<DataQualityStatus>();

        await foreach (var entity in
            _dataQualityStatusTableClient.QueryAsync<DataQualityStatus>())
        {
            results.Add(entity);
        }

        return results;
    }

    public async Task UpdateDataQualityAsync(DataQualityStatus.DataQualityEstablishmentType establishmentType, DateTime lastUpdated)
    {
        var rowKey = ((int) establishmentType).ToString();
        var response = await _dataQualityStatusTableClient.GetEntityIfExistsAsync<DataQualityStatus>(DataQualityStatusPartitionKey, rowKey);

        if (response.HasValue)
        {
            var entity = response.Value;
            entity.LastUpdated = lastUpdated;
            await _dataQualityStatusTableClient.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace);
        }
    }

    public async Task UpdateDataQualityDataOwnerDetailsAsync(DataQualityStatus.DataQualityEstablishmentType establishmentType, string dataOwnerName, string dataOwnerEmail)
    {
        var rowKey = ((int) establishmentType).ToString();
        var response = await _dataQualityStatusTableClient.GetEntityIfExistsAsync<DataQualityStatus>(DataQualityStatusPartitionKey, rowKey);

        if (response.HasValue)
        {
            var entity = response.Value;
            entity.DataOwner = dataOwnerName;
            entity.Email = dataOwnerEmail;
            await _dataQualityStatusTableClient.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace);
        }
    }

    private List<DataQualityStatus> GetSeedData() => new()
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
}
