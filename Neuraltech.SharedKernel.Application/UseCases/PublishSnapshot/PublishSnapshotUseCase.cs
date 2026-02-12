using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Application.UseCases.Base;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Contracts;

namespace Neuraltech.SharedKernel.Application.UseCases.PublishSnapshot
{
    public abstract class PublishSnapshotUseCase<TEntity, TSnapshot, TIntegrationSnapshot> : BaseUseCase<Guid>
        where TSnapshot : class
        where TIntegrationSnapshot : class
        where TEntity : AggregateRoot, ISnapshotable<TEntity, TSnapshot>
    {
        private readonly ISnapshotPublisher _snapshotPublisher;
        private readonly IFindByIdRepository<TEntity> _repository;

        protected abstract TIntegrationSnapshot MapSnapshot(TSnapshot snapshot);

        public PublishSnapshotUseCase(
            IFindByIdRepository<TEntity> repository,
            ISnapshotPublisher snapshotPublisher,
            ILogger logger
        ) : base(logger)
        {
            _repository = repository;
            _snapshotPublisher = snapshotPublisher;
        }
        protected override async ValueTask<UseCaseResponse<Unit>> ExecuteLogic(Guid id)
        {
            var entity = await _repository.Find(id);
            var snapshot = entity?.ToSnapshot();
            var integrationSnapshot = snapshot is not null ? 
                MapSnapshot(snapshot!) : null;


            await _snapshotPublisher.Publish(id,integrationSnapshot);

            return UseCaseResponse.Empty();
        }
    }
}
