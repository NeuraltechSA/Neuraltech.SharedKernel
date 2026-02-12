using Microsoft.EntityFrameworkCore;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Base.Criteria;
using Neuraltech.SharedKernel.Domain.Contracts;
using ZiggyCreatures.Caching.Fusion;

namespace Neuraltech.SharedKernel.Infraestructure.Persistence.Cached
{
    public abstract class BaseCachedRepository<TEntity, TCriteria, TSnapshot>
        : IRepository<TEntity, TCriteria>
        where TEntity : AggregateRoot, ISnapshotable<TEntity, TSnapshot>
        where TCriteria : BaseCriteria<TCriteria>
        where TSnapshot : class
    {
        protected readonly IRepository<TEntity, TCriteria> _repository;
        protected readonly IFusionCache _cache;
        protected abstract string CacheName { get; }
        protected virtual string[] CollectionTags => ["collection"];

        public BaseCachedRepository(
            IRepository<TEntity, TCriteria> repository,
            IFusionCacheProvider cacheProvider
        )
        {
            _repository = repository;
            _cache = cacheProvider.GetCache(CacheName);
        }

        protected virtual Func<
            FusionCacheFactoryExecutionContext<TSnapshot?>,
            CancellationToken,
            Task<TSnapshot?>
        > GetFindByIdCacheFactory(Guid id)
        {
            return async (ctx, ct) =>
            {
                var entity = await _repository.Find(id);
                return entity?.ToSnapshot();
            };
        }

        protected virtual Action<FusionCacheEntryOptions> GetFindByIdCacheOptionsFactory()
        {
            return options => options.SetDuration(TimeSpan.FromMinutes(10));
        }

        protected virtual Func<
            FusionCacheFactoryExecutionContext<long>,
            CancellationToken,
            Task<long>
        > GetCountCacheFactory(TCriteria criteria)
        {
            return async (ctx, ct) =>
            {
                return await _repository.Count(criteria);
            };
        }

        protected virtual Action<FusionCacheEntryOptions> GetCountCacheOptionsFactory()
        {
            return options => options.SetDuration(TimeSpan.FromMinutes(10));
        }

        protected virtual Func<
            FusionCacheFactoryExecutionContext<IEnumerable<TSnapshot>>,
            CancellationToken,
            Task<IEnumerable<TSnapshot>>
        > GetFindCriteriaCacheFactory(TCriteria criteria)
        {
            return async (ctx, ct) =>
            {
                var items = await _repository.Find(criteria);
                return items.Select(i => i.ToSnapshot());
            };
        }

        protected virtual Action<FusionCacheEntryOptions> GetFindCriteriaCacheOptionsFactory()
        {
            return options => options.SetDuration(TimeSpan.FromMinutes(10));
        }

        private async ValueTask InvalidateEntityCache(TEntity entity)
        {
            await _cache.RemoveAsync(entity.Id.Value.ToString());
        }

        private async ValueTask InvalidateCollectionCache()
        {
            foreach (var tag in CollectionTags)
            {
                await _cache.RemoveByTagAsync(tag);
            }
        }

        public async ValueTask<long> Count(TCriteria criteria)
        {
            return await _cache.GetOrSetAsync(
                $"count:{criteria}",
                GetCountCacheFactory(criteria),
                GetCountCacheOptionsFactory(),
                tags: CollectionTags
            );
        }

        public async ValueTask Create(TEntity entity)
        {
            await InvalidateEntityCache(entity);
            await InvalidateCollectionCache();
            await _repository.Create(entity);
        }

        public async ValueTask Delete(TEntity entity)
        {
            await InvalidateEntityCache(entity);
            await InvalidateCollectionCache();
            await _repository.Delete(entity);
        }

        public async ValueTask<IEnumerable<TEntity>> Find(TCriteria criteria)
        {
            var items = await _cache.GetOrSetAsync(
                criteria.ToString(),
                GetFindCriteriaCacheFactory(criteria),
                GetFindCriteriaCacheOptionsFactory(),
                tags: CollectionTags
            );
            return items.Select(i => TEntity.FromSnapshot(i));
        }

        public async ValueTask<TEntity?> Find(Guid id)
        {
            var snapshot = await _cache.GetOrSetAsync(
                id.ToString(),
                GetFindByIdCacheFactory(id),
                GetFindByIdCacheOptionsFactory()
            );

            return snapshot is not null ? TEntity.FromSnapshot(snapshot) : null;
        }

        public async ValueTask Update(TEntity entity)
        {
            await InvalidateEntityCache(entity);
            await InvalidateCollectionCache();
            await _repository.Update(entity);
        }
    }
}
