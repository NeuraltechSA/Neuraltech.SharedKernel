using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Contracts;

namespace Neuraltech.SharedKernel.Application.Projectors
{
    public abstract class SnapshotProjector<TSnapshot, TEntity> : ISnapshotProjector<TSnapshot>
        where TSnapshot : Snapshot
        where TEntity : AggregateRoot
    {
        private readonly IFindByIdRepository<TEntity> _findByIdRepository;
        private readonly ISaveRepository<TEntity> _saveRepository;  
        private readonly IDeleteRepository<TEntity> _deleteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public SnapshotProjector(
            IFindByIdRepository<TEntity> findByIdRepository,
            ISaveRepository<TEntity> saveRepository,
            IDeleteRepository<TEntity> deleteRepository,
            IUnitOfWork unitOfWork,
            ILogger logger
        )
        {
            _findByIdRepository = findByIdRepository;
            _saveRepository = saveRepository;
            _deleteRepository = deleteRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        private async ValueTask Delete(TSnapshot snapshot)
        {
            _logger.LogInformation("Projecting deletion snapshot {0} to entity {1}", typeof(TSnapshot).FullName, typeof(TEntity).FullName);

            var entity = await ProjectEntity(snapshot);
            if ((await _findByIdRepository.Find(entity.Id.Value)) is not null)
            {
                await _deleteRepository.Delete(entity);
            }
            
            _logger.LogInformation("Snapshot deletion {0} to entity {1} succesfully projected", typeof(TSnapshot).FullName, typeof(TEntity).FullName);

        }

        private async ValueTask Save(TSnapshot snapshot)
        {

            _logger.LogInformation("Projecting snapshot {0} to entity {1}", typeof(TSnapshot).FullName, typeof(TEntity).FullName);
            var entity = await ProjectEntity(snapshot);
            await _saveRepository.Save(entity);
            _logger.LogInformation("Snapshot {0} to entity {1} succesfully projected", typeof(TSnapshot).FullName, typeof(TEntity).FullName);
        }

        public abstract ValueTask<TEntity> ProjectEntity(TSnapshot snapshot);

        public async ValueTask ApplyProjection(TSnapshot snapshot)
        {
            if (!snapshot.IsDeleted)
            {
                await Save(snapshot);
            }
            else
            {
                await Delete(snapshot);
            }
            await _unitOfWork.SaveChangesAndFlushEvents();
        }


        public async Task ConsumeAsync(TSnapshot snapshot)
        {
            await ApplyProjection(snapshot);
        }


    }
}
