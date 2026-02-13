using Microsoft.EntityFrameworkCore;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Base.Criteria;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Services;
using ZiggyCreatures.Caching.Fusion;

namespace Neuraltech.SharedKernel.Infraestructure.Persistence.InMemory
{
    /// <summary>
    /// Repositorio base en memoria usando FusionCache con Named Caches.
    /// Optimizado para datos volátiles con alto rate de lectura/escritura.
    /// Cada repositorio usa su propio named cache aislado.
    /// Ideal para: Sessions, Tracking data.
    /// </summary>
    /// <typeparam name="TEntity">Entidad de dominio</typeparam>
    /// <typeparam name="TCriteria">Criterio de búsqueda</typeparam>
    public abstract class IndexedInMemoryRepository<TEntity, TCriteria>
        : IRepository<TEntity, TCriteria>
        where TEntity : Entity
        where TCriteria : BaseCriteria<TCriteria>
    {
        private readonly IFusionCacheProvider _cacheProvider;
        private readonly LinqCriteriaConverter _criteriaConverter;

        protected abstract string CacheName { get; }
        private IFusionCache Cache => _cacheProvider.GetCache(CacheName);
        protected virtual FusionCacheEntryOptions DefaultCacheOptions =>
            new FusionCacheEntryOptions
            {
                Duration = TimeSpan.FromMinutes(5), // TTL corto por defecto
                Size = 1,
                IsFailSafeEnabled = true,
                FailSafeMaxDuration = TimeSpan.FromMinutes(30),
                FailSafeThrottleDuration = TimeSpan.FromSeconds(10),
            };

        protected IndexedInMemoryRepository(IFusionCacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
            _criteriaConverter = new LinqCriteriaConverter();
        }

        protected abstract Guid GetEntityId(TEntity entity);
        protected abstract string EntityCachePrefix { get; }
        protected virtual string IndexesCacheKey => $"keys/{EntityCachePrefix}";

        protected virtual string GetEntityCacheKey(Guid id) => $"{EntityCachePrefix}/{id}";

        public virtual async ValueTask Create(TEntity entity)
        {
            var id = GetEntityId(entity);
            var cacheKey = GetEntityCacheKey(id);

            var existing = await Cache.TryGetAsync<TEntity>(cacheKey);
            if (existing.HasValue)
            {
                throw new InvalidOperationException(
                    $"Entity with ID '{id}' already exists in cache. Use Update instead."
                );
            }

            // Guardar entidad
            await Cache.SetAsync(cacheKey, entity, DefaultCacheOptions);
            await AddToIndexes(id);
        }

        public virtual async ValueTask Update(TEntity entity)
        {
            var id = GetEntityId(entity);
            var cacheKey = GetEntityCacheKey(id);

            await Cache.SetAsync(cacheKey, entity, DefaultCacheOptions);
        }

        public virtual async ValueTask Delete(TEntity entity)
        {
            var id = GetEntityId(entity);
            var cacheKey = GetEntityCacheKey(id);

            // Eliminar entidad
            await Cache.RemoveAsync(cacheKey);

            await RemoveFromIndexes(id);
        }

        /// <summary>
        /// Busca una entidad por ID
        /// </summary>
        public virtual async ValueTask<TEntity?> Find(Guid id)
        {
            var cacheKey = GetEntityCacheKey(id);
            var result = await Cache.TryGetAsync<TEntity>(cacheKey);
            return result.HasValue ? result.Value : null;
        }

        /// <summary>
        /// Busca entidades que cumplan con el criterio
        /// Operación costosa en InMemory: itera todas las entidades
        /// </summary>
        public virtual async ValueTask<IEnumerable<TEntity>> Find(TCriteria criteria)
        {
            // Obtener todas las entidades
            var entities = await GetAll();

            // Aplicar filtros en memoria usando LinqCriteriaConverter
            var query = entities.AsQueryable();
            var filteredQuery = _criteriaConverter.Apply(criteria, query);

            return filteredQuery.ToList();
        }

        /// <summary>
        /// Cuenta entidades que cumplen el criterio
        /// </summary>
        public virtual async ValueTask<long> Count(TCriteria criteria)
        {
            var allEntities = await GetAll();

            var query = allEntities.AsQueryable();
            return await _criteriaConverter.Apply(criteria, query).LongCountAsync();
        }

        /// <summary>
        /// Obtiene todas las entidades desde el cache
        /// </summary>
        protected virtual async ValueTask<IEnumerable<TEntity>> GetAll()
        {
            var indexes = await GetIndexes();
            var entities = new List<TEntity>();

            foreach (var id in indexes)
            {
                var entity = await Find(id);
                if (entity == null)
                    continue;

                entities.Add(entity);
            }

            return entities;
        }

        protected virtual async ValueTask<HashSet<Guid>> GetIndexes()
        {
            var indexes = await Cache.TryGetAsync<HashSet<Guid>>(
                IndexesCacheKey,
                DefaultCacheOptions
            );
            return indexes.GetValueOrDefault([]);
        }

        protected virtual async ValueTask AddToIndexes(Guid id)
        {
            var indexes = await GetIndexes();
            indexes.Add(id);
            await SaveIndexes(indexes);
        }

        protected virtual async ValueTask RemoveFromIndexes(Guid id)
        {
            var indexes = await GetIndexes();
            indexes.Remove(id);
            await SaveIndexes(indexes);
        }

        public virtual async ValueTask SaveIndexes(HashSet<Guid> keys)
        {
            await Cache.SetAsync(IndexesCacheKey, keys, DefaultCacheOptions);
        }

        public virtual async ValueTask Save(TEntity entity)
        {
            var id = GetEntityId(entity);
            var cacheKey = GetEntityCacheKey(id);

            var existing = await Cache.TryGetAsync<TEntity>(cacheKey);
            if (!existing.HasValue)
            {
                await AddToIndexes(id);
            }

            await Cache.SetAsync(cacheKey, entity, DefaultCacheOptions);
        }
    }
}
