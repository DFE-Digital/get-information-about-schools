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
    public class CachedGroupReadRepository : CachedRepositoryBase<GroupCollection>, ICachedGroupReadRepository
    {
        private readonly IApplicationDbContextFactory _dbContextFactory;
        private readonly IApplicationDbContextFactory _inMemoryDbContextFactory;
        private readonly IGroupReadRepository _repo;

        public const string RelationshipKey = "group";

        public CachedGroupReadRepository(IGroupReadRepository repo,
            ICacheAccessor cache,
            IInMemoryApplicationDbContextFactory inMemoryDbContextFactory,
            IApplicationDbContextFactory dbContextFactory)
            : base(cache, inMemoryDbContextFactory, dbContextFactory, repo as RepositoryBase<GroupCollection>)
        {
            _repo = repo;
            _inMemoryDbContextFactory = inMemoryDbContextFactory;
            _dbContextFactory = dbContextFactory;
        }

        public override async Task<GroupCollection> GetAsync(int id)
            => await AutoAsync(async () => await _repo.GetAsync(id), Keyify(id), GetRelationshipCacheKey(id));

        public static string GetRelationshipCacheKey(int urn) => string.Concat(RelationshipKey, "-", urn).ToLower();

        public async Task ClearRelationshipCacheAsync(int? urn)
        {
            if (urn.HasValue) await CacheAccessor.ClearRelatedCacheKeysAsync(GetRelationshipCacheKey(urn.Value));
        }

        protected override DbSet<GroupCollection> GetEntityDbSet(IApplicationDbContext dbContext) => dbContext.Groups;
    }
}
