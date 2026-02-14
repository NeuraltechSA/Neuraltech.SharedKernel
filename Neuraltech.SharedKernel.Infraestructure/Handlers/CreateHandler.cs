using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neuraltech.SharedKernel.Application.UseCases.Create;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Exceptions;
using Neuraltech.SharedKernel.Infraestructure.Attributes;
using Neuraltech.SharedKernel.Infraestructure.Extensions;

namespace Neuraltech.SharedKernel.Infraestructure.Handlers
{
    public abstract class CreateHandler<TRequest, TEntity> 
        (
            CreateUseCase<TEntity> useCase,
            IValidator<TRequest> validator
        ) : CreateHandler<TRequest, TEntity, TEntity>(useCase, validator)
        where TEntity : AggregateRoot
    {

    }

    public abstract class CreateHandler<TRequest, TUseCaseRequest, TEntity>
        (
            CreateUseCase<TUseCaseRequest, TEntity> useCase,
            IValidator<TRequest> validator
        ) : ControllerBase
        where TEntity : AggregateRoot
    {
        CreateUseCase<TUseCaseRequest, TEntity> _useCase = useCase;
        IValidator<TRequest> _validator = validator;

        protected abstract TUseCaseRequest MapUseCaseRequest(TRequest request);

        //[MapException(typeof(IdAlreadyExistsException), StatusCodes.Status409Conflict)]

      
        public virtual async ValueTask<IActionResult> Create(TRequest request)
        {
            await _validator.ValidateAndThrowAsync(request);

            await _useCase.Execute(MapUseCaseRequest(request));
            return NoContent();
        }
    }
}
