using Edubase.Common;
using Edubase.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Edubase.Services
{
    public class GovernorService
    {
        public async Task<IEnumerable<Governor>> GetCurrentByUrn(int urn)
        {
            return await ApplicationDbContext
                .OperationAsync(dc => GetCurrentQuery(dc)
                .Where(x => x.EstablishmentUrn == urn).ToArrayAsync());
        }
        public async Task<IEnumerable<Governor>> GetCurrentByGroupUID(int groupUID)
        {
            return await ApplicationDbContext
                .OperationAsync(dc => GetCurrentQuery(dc)
                .Where(x => x.GroupUID == groupUID).ToArrayAsync());
        }


        public async Task<IEnumerable<Governor>> GetHistoricalByUrn(int urn)
        {
            return await ApplicationDbContext
                .OperationAsync(dc => GetHistoricalQuery(dc)
                .Where(x => x.EstablishmentUrn == urn).ToArrayAsync());
        }


        public async Task<IEnumerable<Governor>> GetHistoricalByGroupUID(int groupUID)
        {
            return await ApplicationDbContext
                .OperationAsync(dc => GetHistoricalQuery(dc)
                .Where(x => x.GroupUID == groupUID).ToArrayAsync());
        }
        /// <summary>
        /// Governors who haven't left yet
        /// </summary>
        /// <param name="dc"></param>
        /// <returns></returns>
        private IQueryable<Governor> GetCurrentQuery(ApplicationDbContext dc)
        {
            var date = DateTime.UtcNow.Date;
            return dc.Governors
                .Include(x => x.AppointingBody).Include(x => x.Role)
                .Where(x => !x.AppointmentEndDate.HasValue || x.AppointmentEndDate > date);
        }

        /// <summary>
        /// Governors who left in the past 12 months
        /// </summary>
        /// <param name="dc"></param>
        /// <returns></returns>
        private IQueryable<Governor> GetHistoricalQuery(ApplicationDbContext dc)
        {
            var date1 = DateTime.UtcNow.Date.AddYears(-1);
            var date2 = DateTime.UtcNow.Date;
            return dc.Governors.Include(x => x.AppointingBody).Include(x => x.Role)
                .Where(x => x.AppointmentEndDate != null
                && x.AppointmentEndDate.Value > date1
                && x.AppointmentEndDate < date2);
        }
    }
}
