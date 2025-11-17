using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Application.UseCases.Base;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace Neuraltech.SharedKernel.Application.UseCases.Create
{
    public abstract class CreateUseCase<TRequest, TEntity>(
        ILogger logger,
        ICreateRepository<TEntity> repository,
        IEventBus eventBus,
        IUnitOfWork unitOfWork
    ) : CreateUseCase<TRequest, TEntity, TEntity>(logger, repository, eventBus, unitOfWork)
        where TEntity : AggregateRoot
    { 
    }

    public abstract class CreateUseCase<TRequest, TBaseEntity, TEntity>(
        ILogger logger,
        ICreateRepository<TBaseEntity> repository,
        IEventBus eventBus,
        IUnitOfWork unitOfWork
    ) : BaseUseCase<TRequest>(logger)
        where TBaseEntity : AggregateRoot
        where TEntity : TBaseEntity
    {
        private readonly ICreateRepository<TBaseEntity> _repository = repository;
        protected readonly IEventBus _eventBus = eventBus;
        protected readonly IUnitOfWork _unitOfWork = unitOfWork;
        
        protected abstract ValueTask<TEntity> ProcessRequest(TRequest request);

        protected override async ValueTask<UseCaseResponse<Unit>> ExecuteLogic(TRequest request)
        {
            var entity = await ProcessRequest(request);
            try
            {
                await _repository.Create(entity);
                await _eventBus.Publish(entity.PullDomainEvents());
                await _unitOfWork.SaveChangesAsync();

                return UseCaseResponse.Empty();
            }
            catch(ArgumentException e)
            {
                if(e.Message.StartsWith("An item with the same key has already been added."))
                {
                    throw IdAlreadyExistsException.CreateFromId(entity.Id.Value);
                }
                throw;
            }

        }
    }
}
