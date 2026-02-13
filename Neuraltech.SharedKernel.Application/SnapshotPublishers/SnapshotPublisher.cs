using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Application.UseCases.Base;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Contracts;

namespace Neuraltech.SharedKernel.Application.SnapshotPublishers
{
    public abstract class SnapshotPublisher<TEntity, TSnapshot> : 
        BaseUseCase<SnapshotPublisherDTO<TEntity>>
        where TSnapshot : Snapshot
        where TEntity : AggregateRoot
    {
        private readonly ISnapshotPublisher _snapshotPublisher;
        //private readonly IFindByIdRepository<TEntity> _repository;

        protected abstract TSnapshot MapSnapshot(TEntity entity);

        public SnapshotPublisher(
            //IFindByIdRepository<TEntity> repository,
            ISnapshotPublisher snapshotPublisher,
            ILogger logger
        ) : base(logger)
        {
            //_repository = repository;
            _snapshotPublisher = snapshotPublisher;
        }

        protected override async ValueTask<UseCaseResponse<Unit>> 
            ExecuteLogic(SnapshotPublisherDTO<TEntity> request)
        {
            //TODO: Should i get fresh result from db or use the one from request?
            //var entity = await _repository.Find(id);
            var publicSnapshot = MapSnapshot(request.Entity);

            if (request.MarkAsDeleted) publicSnapshot.IsDeleted = true;

            await _snapshotPublisher.Publish(
                request.Entity.Id.Value,
                publicSnapshot
            );

            return UseCaseResponse.Empty();
        }
   
    }
}
