using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Base.Criteria;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Services;
using ZiggyCreatures.Caching.Fusion;

namespace Neuraltech.SharedKernel.Infraestructure.Persistence.InMemory
{
    /// <summary>
    /// Repositorio base en memoria usando FusionCache que almacena todas las entidades en una sola clave como lista.
    /// Optimizado para casos donde necesitas filtrar en memoria frecuentemente.
    /// 
    /// Diferencias con IndexedInMemoryRepository:
    /// - IndexedInMemoryRepository: Usa índice + claves individuales. Mejor para Find(id) frecuente.
    /// - InMemoryRepository: Usa lista única. Mejor para Find(criteria) frecuente y menos Find(id).
    /// 
    /// Ventajas:
    /// - Una sola operación de cache para obtener todos los elementos (más rápido para filtros)
    /// - No necesita mantener índice sincronizado
    /// - Menos claves en cache
    /// 
    /// Desventajas:
    /// - Find(id) requiere deserializar toda la lista
    /// - Update/Delete requieren reescribir toda la lista
    /// 
    /// Ideal para: Pequeños conjuntos de datos (<1000 registros) con muchos filtros en memoria.
    /// </summary>
    /// <typeparam name="TEntity">Entidad de dominio</typeparam>
    /// <typeparam name="TCriteria">Criterio de búsqueda</typeparam>
    public abstract class InMemoryRepository<TEntity, TCriteria> : IRepository<TEntity, TCriteria>
        where TEntity : Entity
        where TCriteria : BaseCriteria<TCriteria>
    {
        private readonly IFusionCacheProvider _cacheProvider;
        private readonly LinqCriteriaConverter _criteriaConverter;

        /// <summary>
        /// Nombre del cache aislado para este repositorio
        /// </summary>
        protected abstract string CacheName { get; }

        /// <summary>
        /// Clave única donde se almacena la lista completa de entidades
        /// </summary>
        protected abstract string CollectionCacheKey { get; }

        private IFusionCache Cache => _cacheProvider.GetCache(CacheName);

        protected virtual FusionCacheEntryOptions DefaultCacheOptions =>
            new FusionCacheEntryOptions
            {
                Duration = TimeSpan.FromMinutes(5),
                Size = 1,
                IsFailSafeEnabled = true,
                FailSafeMaxDuration = TimeSpan.FromMinutes(30),
                FailSafeThrottleDuration = TimeSpan.FromSeconds(10)
            };

        protected InMemoryRepository(IFusionCacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
            _criteriaConverter = new LinqCriteriaConverter();
        }

        /// <summary>
        /// Extrae el ID de una entidad
        /// </summary>
        protected abstract Guid GetEntityId(TEntity entity);

        /// <summary>
        /// Obtiene todas las entidades desde el cache.
        /// Esta es la operación fundamental - todo se basa en obtener la lista completa.
        /// </summary>
        protected virtual async ValueTask<List<TEntity>> GetAll()
        {
            var result = await Cache.TryGetAsync<List<TEntity>>(CollectionCacheKey);
            return result.GetValueOrDefault([]);
        }

        /// <summary>
        /// Guarda toda la colección en cache
        /// </summary>
        protected virtual async ValueTask SaveAll(List<TEntity> entities)
        {
            await Cache.SetAsync(CollectionCacheKey, entities, DefaultCacheOptions);
        }

        /// <summary>
        /// Crea una nueva entidad en memoria
        /// </summary>
        public virtual async ValueTask Create(TEntity entity)
        {
            var id = GetEntityId(entity);
            var entities = await GetAll();

            // Verificar si ya existe
            if (entities.Any(e => GetEntityId(e) == id))
            {
                throw new InvalidOperationException(
                    $"Entity with ID '{id}' already exists in cache '{CacheName}'. Use Update instead.");
            }

            // Agregar y guardar
            entities.Add(entity);
            await SaveAll(entities);
        }

        /// <summary>
        /// Actualiza una entidad existente en memoria
        /// </summary>
        public virtual async ValueTask Update(TEntity entity)
        {
            var id = GetEntityId(entity);
            var entities = await GetAll();

            // Buscar y reemplazar
            var index = entities.FindIndex(e => GetEntityId(e) == id);
            if (index == -1)
            {
                throw new InvalidOperationException(
                    $"Entity with ID '{id}' not found in cache '{CacheName}'. Use Create instead.");
            }

            entities[index] = entity;
            await SaveAll(entities);
        }

        /// <summary>
        /// Elimina una entidad de memoria
        /// </summary>
        public virtual async ValueTask Delete(TEntity entity)
        {
            var id = GetEntityId(entity);
            var entities = await GetAll();

            // Remover y guardar
            var removed = entities.RemoveAll(e => GetEntityId(e) == id);
            if (removed == 0)
            {
                throw new InvalidOperationException(
                    $"Entity with ID '{id}' not found in cache '{CacheName}'.");
            }

            await SaveAll(entities);
        }

        /// <summary>
        /// Busca una entidad por ID.
        /// Nota: Requiere deserializar toda la lista, menos eficiente que IndexedInMemoryRepository.
        /// </summary>
        public virtual async ValueTask<TEntity?> Find(Guid id)
        {
            var entities = await GetAll();
            return entities.FirstOrDefault(e => GetEntityId(e) == id);
        }

        /// <summary>
        /// Busca entidades que cumplan con el criterio.
        /// Optimizado para este caso de uso - solo una operación de cache.
        /// </summary>
        public virtual async ValueTask<IEnumerable<TEntity>> Find(TCriteria criteria)
        {
            // Una sola operación de cache para obtener todos
            var entities = await GetAll();

            // Aplicar filtros en memoria
            var query = entities.AsQueryable();
            var filteredQuery = _criteriaConverter.Apply(criteria, query);

            return filteredQuery.ToList();
        }

        /// <summary>
        /// Cuenta entidades que cumplen el criterio
        /// </summary>
        public virtual async ValueTask<long> Count(TCriteria criteria)
        {
            var entities = await GetAll();
            var query = entities.AsQueryable();
            return await _criteriaConverter.Apply(criteria, query).LongCountAsync();
        }

        /// <summary>
        /// Limpia toda la colección
        /// </summary>
        public virtual async ValueTask ClearAll()
        {
            await Cache.RemoveAsync(CollectionCacheKey);
        }
    }
}
