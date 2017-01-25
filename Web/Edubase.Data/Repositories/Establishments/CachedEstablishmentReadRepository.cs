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

namespace Edubase.Data.Repositories.Establishments
{
    public class CachedEstablishmentReadRepository : CachedRepositoryBase<Establishment>, ICachedEstablishmentReadRepository
    {
        private readonly IApplicationDbContextFactory _dbContextFactory;
        private readonly IApplicationDbContextFactory _inMemoryDbContextFactory;
        private readonly IEstablishmentReadRepository _repo;

        public const string RelationshipKey = "establishment";

        public CachedEstablishmentReadRepository(IEstablishmentReadRepository repo, 
            ICacheAccessor cache,
            IInMemoryApplicationDbContextFactory inMemoryDbContextFactory,
            IApplicationDbContextFactory dbContextFactory) 
            : base(cache, inMemoryDbContextFactory, dbContextFactory, repo as RepositoryBase<Establishment>) // todo: remove cast!
        {
            _repo = repo;
            _inMemoryDbContextFactory = inMemoryDbContextFactory;
            _dbContextFactory = dbContextFactory;
        }

        public override async Task<Establishment> GetAsync(int urn) 
            => await AutoAsync(async () => await _repo.GetAsync(urn), Keyify(urn), GetRelationshipCacheKey(urn));
        
        public async Task<int?> GetStatusAsync(int urn)
            => await AutoAsync(async () => await _repo.GetStatusAsync(urn), Keyify(urn), GetRelationshipCacheKey(urn));

        public async Task<int[]> GetUrns(int skip, int take)
        {
            return await _repo.GetUrns(skip, take);
        }

        public static string GetRelationshipCacheKey(int urn) => string.Concat(RelationshipKey, "-", urn).ToLower();

        public async Task ClearRelationshipCacheAsync(int? urn)
        {
            if(urn.HasValue) await CacheAccessor.ClearRelatedCacheKeysAsync(GetRelationshipCacheKey(urn.Value));
        }

        protected override DbSet<Establishment> GetEntityDbSet(IApplicationDbContext dbContext) => dbContext.Establishments;

    }
}
