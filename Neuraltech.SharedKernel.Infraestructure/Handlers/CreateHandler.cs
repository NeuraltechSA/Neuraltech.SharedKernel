using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neuraltech.SharedKernel.Application.UseCases.Create;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Exceptions;
using Neuraltech.SharedKernel.Infraestructure.Attributes;

namespace Neuraltech.SharedKernel.Infraestructure.Handlers
{
    public abstract class CreateHandler<TRequest, TEntity> 
        (CreateUseCase<TEntity> useCase) : CreateHandler<TRequest, TEntity, TEntity>(useCase)
        where TEntity : AggregateRoot
    {

    }

    public abstract class CreateHandler<TRequest, TUseCaseRequest, TEntity>
        (CreateUseCase<TUseCaseRequest, TEntity> useCase) : ControllerBase
        where TEntity : AggregateRoot
    {
        CreateUseCase<TUseCaseRequest, TEntity> _useCase = useCase;

        protected abstract TUseCaseRequest MapUseCaseRequest(TRequest request);

        [MapException(typeof(IdAlreadyExistsException), StatusCodes.Status409Conflict)]
        public virtual async ValueTask<IActionResult> Create(TRequest request)
        {
            await _useCase.Execute(MapUseCaseRequest(request));
            return NoContent();
        }
    }
}
