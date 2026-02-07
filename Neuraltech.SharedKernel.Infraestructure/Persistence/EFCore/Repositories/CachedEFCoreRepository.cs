using Microsoft.EntityFrameworkCore;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Base.Criteria;
using Neuraltech.SharedKernel.Infraestructure.Persistence.Contracts;
using Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Models;
using Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Services;
using ZiggyCreatures.Caching.Fusion;

namespace Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Repositories
{
    /// <summary>
    /// Repositorio base con soporte para cache (FusionCache) y etiquetas (Tags).
    /// </summary>
    public abstract class CachedEFCoreRepository<TEntity, TModel, TCriteria>
        : BaseEFCoreRepository<TEntity, TModel, TCriteria>
        where TEntity : AggregateRoot
        where TModel : BaseEFCoreModel
        where TCriteria : BaseCriteria<TCriteria>
    {
        protected readonly IFusionCache _cache;

        /// <summary>
        /// Prefijo para las keys de cache y tag de la colección (ej: "posts").
        /// </summary>
        protected abstract string CacheKeyPrefix { get; }
        protected virtual string[] EntityCacheTags => [CacheKeyPrefix];
        protected virtual string[] CollectionCacheTags =>
            [CacheKeyPrefix, $"{CacheKeyPrefix}:collection"];

        protected string GetEntityCacheKey(Guid id) => $"{CacheKeyPrefix}:{id}";

        protected string GetCriteriaCacheKey(TCriteria criteria) => $"{CacheKeyPrefix}:{criteria}";

        protected string GetCountCacheKey(TCriteria criteria) =>
            $"{CacheKeyPrefix}:count:{criteria}";

        protected CachedEFCoreRepository(
            DbContext context,
            LinqCriteriaConverter linqCriteriaConverter,
            IMapper<TEntity, TModel> modelParser,
            IFusionCache cache
        )
            : base(context, linqCriteriaConverter, modelParser)
        {
            _cache = cache;
        }

        #region Cache Helpers

        protected async ValueTask RemoveCollectionCache()
        {
            foreach (var tag in CollectionCacheTags)
            {
                await _cache.RemoveByTagAsync(tag);
            }
        }

        #endregion

        public override async ValueTask Create(TEntity entity)
        {
            await base.Create(entity);
            await RemoveCollectionCache();
        }

        public override async ValueTask Update(TEntity entity)
        {
            await base.Update(entity);
            await _cache.RemoveAsync(GetEntityCacheKey(entity.Id.Value));
            await RemoveCollectionCache();
        }

        public override async ValueTask Delete(TEntity entity)
        {
            await base.Delete(entity);
            await _cache.RemoveAsync(GetEntityCacheKey(entity.Id.Value));
            await RemoveCollectionCache();
        }

        public override async ValueTask<long> Count(TCriteria criteria)
        {
            return await _cache.GetOrSetAsync(
                GetCountCacheKey(criteria),
                async _ => await base.Count(criteria),
                (options, ctx) => options.SetDuration(TimeSpan.FromMinutes(10))
            );
        }
    }
}
