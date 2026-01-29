using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Application.UseCases.Base;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Domain.Services;

namespace Neuraltech.SharedKernel.Application.UseCases.Delete
{
    /*
     
        {
            entity.Delete();
            return ValueTask.CompletedTask;
        }
     */


    public abstract class DeleteUseCase<TEntity>(
        ILogger logger,
        IFindByIdRepository<TEntity> findByIdRepository,
        IDeleteRepository<TEntity> repository,
        IEventBus eventBus,
        IUnitOfWork unitOfWork
    ) : BaseUseCase<Guid>(logger)
        where TEntity : AggregateRoot
    {
        private readonly IDeleteRepository<TEntity> _repository = repository;
        private readonly IEventBus _eventBus = eventBus;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IFindByIdRepository<TEntity> _findByIdRepository = findByIdRepository;

        protected virtual ValueTask ProcessRequest(TEntity entity)
        {
            return ValueTask.CompletedTask;
        }

        protected override async ValueTask<UseCaseResponse<Unit>> ExecuteLogic(Guid id)
        {
            var entity = await _findByIdRepository.Find(id);
            if (entity is null)
            {
                _logger.LogWarning($"Entity of type {typeof(TEntity).Name} with id {id} not found");
                return UseCaseResponse.Empty();
            }

            _logger.LogInformation($"Deleting entity of type {typeof(TEntity).Name} with id {id}");

            await ProcessRequest(entity!);

            if (entity is IDeletable deletableEntity) deletableEntity.Delete();

            await _repository.Delete(entity!);
            await _eventBus.Publish(entity!.PullDomainEvents());
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Entity of type {typeof(TEntity).Name} with id {id} deleted successfully");

            return UseCaseResponse.Empty();
        }
    }
}
