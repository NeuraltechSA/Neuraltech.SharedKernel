using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Neuraltech.SharedKernel.Application.UseCases.Update;
using Neuraltech.SharedKernel.Domain.Base;

namespace Neuraltech.SharedKernel.Infraestructure.Handlers
{
    public abstract class UpdateHandler<TRequest, TUseCaseRequest, TEntity>(
        UpdateUseCase<TUseCaseRequest, TEntity> useCase,
        IValidator<TRequest> validator
    ) : ControllerBase
        where TEntity : AggregateRoot
        where TUseCaseRequest : UpdateDTO
    {
        private readonly UpdateUseCase<TUseCaseRequest, TEntity> _useCase = useCase;
        private readonly IValidator<TRequest> _validator = validator;

        protected abstract TUseCaseRequest MapUseCaseRequest(TRequest request, Guid id);

        public virtual async ValueTask<IActionResult> Update(string id, TRequest request)
        {
            await _validator.ValidateAndThrowAsync(request);

            await _useCase.Execute(MapUseCaseRequest(request, new Guid(id)));
            return NoContent();
        }
    }
}
