using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Application.UseCases.Base;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Domain.Exceptions;


namespace Neuraltech.SharedKernel.Application.UseCases.Create
{
    public abstract class CreateUseCase<TEntity>(
        ILogger logger,
        ICreateRepository<TEntity> repository,
        IEventBus eventBus,
        IUnitOfWork unitOfWork
    ) : CreateUseCase<TEntity, TEntity>(
        logger,
        repository,
        eventBus,
        unitOfWork
    ) where TEntity : AggregateRoot
    {
        protected override ValueTask<TEntity> ProcessRequest(TEntity request)
        {
            return ValueTask.FromResult(request);
        }
    }

    public abstract class CreateUseCase<TRequest, TEntity>(
        ILogger logger,
        ICreateRepository<TEntity> repository,
        IEventBus eventBus,
        IUnitOfWork unitOfWork
    ) : BaseUseCase<TRequest>(logger)
        where TEntity : AggregateRoot
    {
        private readonly ICreateRepository<TEntity> _repository = repository;
        protected readonly IEventBus _eventBus = eventBus;
        protected readonly IUnitOfWork _unitOfWork = unitOfWork;
        
        protected abstract ValueTask<TEntity> ProcessRequest(TRequest request);

        protected override async ValueTask<UseCaseResponse<Unit>> ExecuteLogic(TRequest request)
        {
            var entity = await ProcessRequest(request);
            await _repository.Create(entity);
            await _eventBus.Publish(entity.PullDomainEvents());
            await _unitOfWork.SaveChangesAsync();

            return UseCaseResponse.Empty();
        }
    }
}
