using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neuraltech.SharedKernel.Application.UseCases.Update;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Infraestructure.Attributes;

namespace Neuraltech.SharedKernel.Infraestructure.Handlers
{
    public abstract class UpdateHandler<TRequest, TUseCaseRequest, TEntity>
        (UpdateUseCase<TUseCaseRequest, TEntity> useCase) : ControllerBase
        where TEntity : AggregateRoot
        where TUseCaseRequest : UpdateDTO
    {
        UpdateUseCase<TUseCaseRequest, TEntity> _useCase = useCase;

        protected abstract TUseCaseRequest MapUseCaseRequest(TRequest request, Guid id);

        [MapException(typeof(EntityToUpdateNotFoundException), StatusCodes.Status404NotFound)]
        public virtual async ValueTask<IActionResult> Update(string id, TRequest request)
        {
            await _useCase.Execute(MapUseCaseRequest(request, new Guid(id)));
            return NoContent();
        }
    }
}
