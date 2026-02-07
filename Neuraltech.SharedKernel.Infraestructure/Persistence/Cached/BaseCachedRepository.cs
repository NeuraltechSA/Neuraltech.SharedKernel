using Microsoft.EntityFrameworkCore;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Base.Criteria;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Models;
using Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Repositories;
using ZiggyCreatures.Caching.Fusion;

namespace Neuraltech.SharedKernel.Infraestructure.Persistence.CachedFCore
{
    /// <summary>
    /// Repositorio base que implementa el patrón Decorator para agregar cache a un repositorio EF Core existente.
    /// Utiliza FusionCache para proporcionar cache de dos niveles (L1: Memory, L2: Redis) con características avanzadas.
    /// </summary>
    /// <typeparam name="TEntity">Entidad de dominio</typeparam>
    /// <typeparam name="TModel">Modelo de persistencia EF Core</typeparam>
    /// <typeparam name="TCriteria">Criterio de búsqueda</typeparam>
    public abstract class BaseCachedRepository<TEntity, TModel, TCriteria>(
            BaseEFCoreRepository<TEntity, TModel, TCriteria> repository,
            IFusionCache cachingProvider
        ) : IRepository<TEntity, TCriteria>
        where TEntity : AggregateRoot
        where TModel : BaseEFCoreModel
        where TCriteria : BaseCriteria<TCriteria>
    {
        private readonly BaseEFCoreRepository<TEntity, TModel, TCriteria> _repository = repository;
        private readonly IFusionCache _cachingProvider = cachingProvider;

        /// <summary>
        /// Opciones de cache (duración, prioridad, etc.)
        /// </summary>
        protected abstract FusionCacheEntryOptions CachingOptions { get; }

        /// <summary>
        /// Prefijo para las claves de cache
        /// </summary>
        protected abstract string CacheKeyPrefix { get; }

        /// <summary>
        /// Genera la clave de cache para una entidad
        /// </summary>
        protected virtual string GetCacheKey(TEntity entity)
        {
            // Entity es la clase base que tiene Id de tipo ValueObject
            // Necesitamos acceder al valor del ID de forma genérica
            var entityType = entity.GetType();
            var idProperty = entityType.GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException($"Entity {entityType.Name} does not have an Id property");
            
            var idValueObject = idProperty.GetValue(entity);
            if (idValueObject == null)
                throw new InvalidOperationException($"Entity {entityType.Name} has a null Id");

            // El ValueObject tiene una propiedad Value
            var valueProperty = idValueObject.GetType().GetProperty("Value");
            if (valueProperty == null)
                throw new InvalidOperationException($"Id ValueObject does not have a Value property");

            var idValue = valueProperty.GetValue(idValueObject);
            return $"{CacheKeyPrefix}:{idValue}";
        }

        /// <summary>
        /// Genera la clave de cache para un ID
        /// </summary>
        protected virtual string GetCacheKey(Guid id)
        {
            return $"{CacheKeyPrefix}:{id}";
        }

        /// <summary>
        /// Genera la clave de cache para criterios de búsqueda
        /// </summary>
        protected virtual string GetCacheKey(TCriteria criteria)
        {
            return $"{CacheKeyPrefix}:criteria:{criteria.GetHashCode()}";
        }

        /// <summary>
        /// Crea una nueva entidad en la base de datos y la almacena en cache (Write-Through)
        /// </summary>
        public virtual async ValueTask Create(TEntity entity)
        {
            // 1. Escribir en base de datos
            await _repository.Create(entity);

            // 2. Escribir en cache (Write-Through)
            await _cachingProvider.SetAsync(GetCacheKey(entity), entity, CachingOptions);
        }

        /// <summary>
        /// Actualiza una entidad en la base de datos y actualiza el cache
        /// </summary>
        public virtual async ValueTask Update(TEntity entity)
        {
            // 1. Actualizar en base de datos
            await _repository.Update(entity);

            // 2. Actualizar cache
            await _cachingProvider.SetAsync(GetCacheKey(entity), entity, CachingOptions);
        }

        /// <summary>
        /// Elimina una entidad de la base de datos y del cache
        /// </summary>
        public virtual async ValueTask Delete(TEntity entity)
        {
            // 1. Eliminar de base de datos
            await _repository.Delete(entity);

            // 2. Invalidar cache
            await _cachingProvider.RemoveAsync(GetCacheKey(entity));
        }

        /// <summary>
        /// Busca una entidad por ID utilizando cache (Cache-Aside/Look-Through)
        /// FusionCache maneja automáticamente:
        /// - Fail-Safe: Si la BD falla, devuelve valor cacheado aunque esté expirado
        /// - Soft/Hard Timeout: Timeouts configurables
        /// - Stampede Prevention: Evita múltiples queries simultáneas
        /// </summary>
        public virtual async ValueTask<TEntity?> Find(Guid id)
        {
            var cacheKey = GetCacheKey(id);

            // GetOrSetAsync: Intenta leer del cache, si no existe ejecuta la factory function
            var entity = await _cachingProvider.GetOrSetAsync(
                cacheKey,
                async _ => await _repository.Find(id), // Factory function (se ejecuta solo si cache miss)
                CachingOptions
            );

            return entity;
        }

        /// <summary>
        /// Busca entidades por criterio con cache opcional
        /// Por defecto no se cachean búsquedas complejas, pero se puede sobrescribir
        /// </summary>
        public virtual async ValueTask<IEnumerable<TEntity>> Find(TCriteria criteria)
        {
            // Opción 1: Sin cache (comportamiento por defecto para búsquedas complejas)
            // Las búsquedas con criterios son difíciles de invalidar correctamente
            if (!ShouldCacheCriteria(criteria))
            {
                return await _repository.Find(criteria);
            }

            // Opción 2: Con cache (si se determina que es seguro cachear)
            var cacheKey = GetCacheKey(criteria);
            
            var entities = await _cachingProvider.GetOrSetAsync(
                cacheKey,
                async _ => await _repository.Find(criteria),
                CachingOptions
            );

            return entities ?? Enumerable.Empty<TEntity>();
        }

        /// <summary>
        /// Count entities matching the criteria
        /// By default, it does not cache, but it can be enabled
        /// </summary>
        public virtual async ValueTask<long> Count(TCriteria criteria)
        {
            // By default, do not cache counts (sensitive to data changes)
            if (!ShouldCacheCriteria(criteria))
            {
                return await _repository.Count(criteria);
            }

            var cacheKey = $"{CacheKeyPrefix}:count:{criteria.GetHashCode()}";
            
            var count = await _cachingProvider.GetOrSetAsync(
                cacheKey,
                async _ => await _repository.Count(criteria),
                CachingOptions
            );

            return count;
        }

        /// <summary>
        /// Determines if a specific criteria should be cached
        /// By default, it returns false (do not cache complex searches)
        /// It can be overridden to enable caching for specific criteria
        /// </summary>
        /// <param name="criteria">Criteria to evaluate</param>
        /// <returns>True if it should be cached, False otherwise</returns>
        protected virtual bool   ShouldCacheCriteria(TCriteria criteria)
        {
            // By default, do not cache criteria (invalidating complexity)
            // Derived classes can override for specific criteria
            return false;
        }

        /// <summary>
        /// Invalidates all cache for the repository
        /// Useful after bulk operations or external updates
        /// </summary>
        protected async ValueTask InvalidateAllCache()
        {
            // FusionCache does not have a native method to delete by prefix
            // It can be implemented by keeping a Set of keys or using Redis directly
            // For now, throw an exception to indicate that it needs to be implemented if necessary
            await Task.CompletedTask;
            throw new NotImplementedException(
                "InvalidateAllCache must be implemented if needed. " +
                "Consider keeping a log of keys or using Redis Scan.");
        }

        /// <summary>
        /// Invalidates the cache of a specific entity by ID
        /// </summary>
        protected async ValueTask InvalidateCache(Guid id)
        {
            var cacheKey = GetCacheKey(id);
            await _cachingProvider.RemoveAsync(cacheKey);
        }

        /// <summary>
        /// Invalidates the cache of a specific entity
        /// </summary>
        protected async ValueTask InvalidateCache(TEntity entity)
        {
            await _cachingProvider.RemoveAsync(GetCacheKey(entity));
        }
    }
}
