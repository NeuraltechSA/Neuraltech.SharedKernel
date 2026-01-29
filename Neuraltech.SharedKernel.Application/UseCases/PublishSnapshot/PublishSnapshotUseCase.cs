using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Application.UseCases.Base;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Domain.Services;

namespace Neuraltech.SharedKernel.Application.UseCases.PublishSnapshot
{
    public class PublishSnapshotUseCase<TEntity, TSnapshot> : BaseUseCase<Guid>
        where TSnapshot : class, IEntitySnapshot
        where TEntity : AggregateRoot,IProjectable<TSnapshot>
    {
        private readonly ISnapshotPublisher _snapshotPublisher;
        private readonly IFindByIdRepository<TEntity> _repository;

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
            //Ensure.NotNull(entity);

            await _snapshotPublisher.Publish(id, entity?.ToSnapshot());

            return UseCaseResponse.Empty();
        }
    }
}
