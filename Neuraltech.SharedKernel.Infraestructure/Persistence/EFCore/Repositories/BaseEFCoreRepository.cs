using Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Models;
using Neuraltech.SharedKernel.Infraestructure.Persistence.Contracts;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Domain.Base.Criteria;
using Neuraltech.SharedKernel.Domain.Base;
using Microsoft.EntityFrameworkCore;
using Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Services;
using System.Data.Common;
using Neuraltech.SharedKernel.Infraestructure.Exceptions;

namespace Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Repositories
{
    /// <summary>
    /// Repositorio base para Entity Framework Core que implementa operaciones CRUD básicas.
    /// Proporciona una implementación genérica que separa las entidades de dominio de los modelos de persistencia.
    /// </summary>
    /// <typeparam name="TEntity">Entidad de dominio</typeparam>
    /// <typeparam name="TModel">Modelo de persistencia de EF Core</typeparam>
    /// <typeparam name="TCriteria">Criterio de búsqueda</typeparam>
    public abstract class BaseEFCoreRepository<TEntity, TModel, TCriteria> : IRepository<TEntity, TCriteria>
        where TEntity : AggregateRoot
        where TModel : BaseEFCoreModel
        where TCriteria : BaseCriteria<TCriteria>
    {
        /// <summary>
        /// Contexto de Entity Framework Core
        /// </summary>
        protected readonly DbContext _context;
        
        /// <summary>
        /// DbSet para operaciones sobre el modelo de persistencia
        /// </summary>
        protected readonly DbSet<TModel> _dbSet;
        
        /// <summary>
        /// Convertidor de criterios de dominio a consultas de Linq
        /// </summary>
        protected readonly LinqCriteriaConverter _linqCriteriaConverter;
        
        /// <summary>
        /// Parser para conversión entre entidades de dominio y modelos de persistencia
        /// </summary>
        protected readonly IMapper<TEntity, TModel> _modelParser;

        /// <summary>
        /// Constructor del repositorio base
        /// </summary>
        /// <param name="context">Contexto de Entity Framework Core</param>
        /// <param name="linqCriteriaConverter">Convertidor de criterios</param>
        /// <param name="modelParser">Parser de modelos</param>
        public BaseEFCoreRepository(DbContext context, LinqCriteriaConverter linqCriteriaConverter, IMapper<TEntity, TModel> modelParser)
        {
            _context = context;
            _dbSet = _context.Set<TModel>();
            _linqCriteriaConverter = linqCriteriaConverter;
            _modelParser = modelParser;
        }

        /// <summary>
        /// Obtiene la query base para las operaciones de lectura.
        /// Puede ser sobrescrito para aplicar filtros por defecto (ej: discriminador de tipo en herencia TPH).
        /// </summary>
        /// <returns>Query base con filtros por defecto aplicados</returns>
        protected virtual IQueryable<TModel> GetBaseQuery()
        {
            return _dbSet;
        }
        
        /// <summary>
        /// Crea una nueva entidad en la base de datos.
        /// La entidad se agrega al contexto con tracking habilitado para permitir el seguimiento de cambios.
        /// </summary>
        /// <param name="entity">Entidad de dominio a crear</param>
        public virtual async ValueTask Create(TEntity entity){
            await _dbSet.AddAsync(_modelParser.MapToModel(entity));
        }

        /// <summary>
        /// Actualiza una entidad existente en la base de datos.
        /// La entidad se marca como modificada con tracking habilitado para permitir el seguimiento de cambios.
        /// </summary>
        /// <param name="entity">Entidad de dominio a actualizar</param>
        public virtual async ValueTask Update(TEntity entity){
            _dbSet.Update(_modelParser.MapToModel(entity));
        }

        /// <summary>
        /// Elimina una entidad de la base de datos.
        /// La entidad se marca para eliminación con tracking habilitado.
        /// </summary>
        /// <param name="entity">Entidad de dominio a eliminar</param>
        public virtual async ValueTask Delete(TEntity entity){
            _dbSet.Remove(_modelParser.MapToModel(entity));
        }

        /// <summary>
        /// Busca una entidad por su identificador.
        /// Utiliza AsNoTracking() para optimizar rendimiento al no requerir seguimiento de cambios
        /// ya que es una operación de solo lectura.
        /// </summary>
        /// <param name="id">Identificador de la entidad</param>
        /// <returns>Entidad encontrada o null si no existe</returns>
        public virtual async ValueTask<TEntity?> Find(Guid id){
            // AsNoTracking() mejora rendimiento y reduce uso de memoria al no rastrear cambios
            var result = await GetBaseQuery().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return result == null ? null : _modelParser.MapToEntity(result);
        }
        /*
                     /*try
            {
                
            }
            catch (DbException ex)
            {
                List<string> sqlStates = ["23505", "1062", "2627"];
                if (sqlStates.Contains(ex.SqlState ?? ""))
                {
                    throw IdAlreadyInDbException.Create(model.Id.ToString());
                }
            }
            catch (Exception ex)
            {

            }*/
        /// <summary>
        /// Busca entidades que cumplan con el criterio especificado.
        /// Utiliza AsNoTracking() para optimizar rendimiento al ser una operación de solo lectura.
        /// </summary>
        /// <param name="criteria">Criterio de búsqueda</param>
        /// <returns>Colección de entidades que cumplen el criterio</returns>
        public virtual async ValueTask<IEnumerable<TEntity>> Find(TCriteria criteria){
            // AsNoTracking() mejora rendimiento al no necesitar seguimiento de cambios para consultas de lectura
            var items = await _linqCriteriaConverter.Apply(criteria, GetBaseQuery()).AsNoTracking().ToListAsync();
            return items.Select(_modelParser.MapToEntity);
        }

        public async virtual ValueTask<long> Count(TCriteria criteria)
        {
            return await _linqCriteriaConverter.Apply(criteria, GetBaseQuery()).LongCountAsync();
        }
    }
}
