using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories.EF
{
    public class SqlDataQualityStatusRepository : IDataQualityStatusRepository
    {
        private const string DataQualityPartitionKey = "DataQuality";
        private readonly FrontEndDbContext _context;

        public SqlDataQualityStatusRepository(FrontEndDbContext context)
        {
            _context = context;
        }

        public async Task<List<DataQualityStatus>> GetAllAsync()
        {
            return await _context.DataQualityStatus.ToListAsync();
        }

        public async Task UpdateDataQualityAsync(DataQualityStatus.DataQualityEstablishmentType establishmentType, DateTime lastUpdated)
        {
            var existing = await _context.DataQualityStatus
                .FindAsync(DataQualityPartitionKey, ((int) establishmentType).ToString());

            if (existing != null)
            {
                existing.LastUpdated = lastUpdated;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateDataQualityDataOwnerDetailsAsync(DataQualityStatus.DataQualityEstablishmentType establishmentType, string dataOwnerName, string dataOwnerEmail)
        {
            var existing = await _context.DataQualityStatus
                .FindAsync(DataQualityPartitionKey, ((int) establishmentType).ToString());

            if (existing != null)
            {
                existing.DataOwner = dataOwnerName;
                existing.Email = dataOwnerEmail;
                await _context.SaveChangesAsync();
            }
        }
    }
}
