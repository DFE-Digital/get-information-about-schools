using Edubase.Common.Cache;
using Edubase.Data.Entity;
using System.Threading.Tasks;
using System;
using System.Linq;
using MoreLinq;
using Edubase.Common;
using System.Text;
using Edubase.Data.DbContext;
using System.Collections.Generic;
using System.Data.Entity;
using Edubase.Data.Repositories.Groups.Abstract;

namespace Edubase.Data.Repositories.Groups
{
    public class CachedEstablishmentGroupReadRepository : CachedRepositoryBase<EstablishmentGroup>, ICachedEstablishmentGroupReadRepository
    {
        private readonly IApplicationDbContextFactory _dbContextFactory;
        private readonly IApplicationDbContextFactory _inMemoryDbContextFactory;
        private readonly IEstablishmentGroupReadRepository _repo;

        public const string RelationshipKey = "establishmentgroup";

        public CachedEstablishmentGroupReadRepository(IEstablishmentGroupReadRepository repo,
            ICacheAccessor cache,
            IInMemoryApplicationDbContextFactory inMemoryDbContextFactory,
            IApplicationDbContextFactory dbContextFactory)
            : base(cache, inMemoryDbContextFactory, dbContextFactory, repo as RepositoryBase<EstablishmentGroup>)
        {
            _repo = repo;
            _inMemoryDbContextFactory = inMemoryDbContextFactory;
            _dbContextFactory = dbContextFactory;
        }

        public override async Task<EstablishmentGroup> GetAsync(int id)
            => await AutoAsync(async () => await _repo.GetAsync(id), Keyify(id), GetRelationshipCacheKey(id));

        public static string GetRelationshipCacheKey(int id) => string.Concat(RelationshipKey, "-", id).ToLower();

        public async Task ClearRelationshipCacheAsync(int? id)
        {
            if (id.HasValue) await CacheAccessor.ClearRelatedCacheKeysAsync(GetRelationshipCacheKey(id.Value));
        }

        protected override DbSet<EstablishmentGroup> GetEntityDbSet(IApplicationDbContext dbContext) => dbContext.EstablishmentGroups;

        public async Task<List<EstablishmentGroup>> GetForUrnAsync(int urn)
            => await AutoAsync(async () => await _repo.GetForUrnAsync(urn), Keyify(urn));

        public async Task<List<EstablishmentGroup>> GetForGroupAsync(int groupUId)
            => await AutoAsync(async () => await _repo.GetForGroupAsync(groupUId), Keyify(groupUId));
    }
}
