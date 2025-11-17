using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neuraltech.SharedKernel.Application.UseCases.Create;
using Neuraltech.SharedKernel.Application.UseCases.Update;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Infraestructure.Attributes;

namespace Neuraltech.SharedKernel.Infraestructure.Handlers
{
    public abstract class UpdateHandler<TRequest, TUseCaseRequest, TEntity>
        (UpdateUseCase<TUseCaseRequest, TEntity> useCase)
    : UpdateHandler<TRequest, TUseCaseRequest, TEntity, TEntity>(useCase)
        where TEntity : AggregateRoot
        where TUseCaseRequest : UpdateDTO
    {

    }

    public abstract class UpdateHandler<TRequest, TUseCaseRequest, TEntity, TBaseEntity>
        (UpdateUseCase<TUseCaseRequest, TBaseEntity, TEntity> useCase) : ControllerBase
        where TBaseEntity : AggregateRoot
        where TEntity : TBaseEntity
        where TUseCaseRequest : UpdateDTO
    {
        UpdateUseCase<TUseCaseRequest, TBaseEntity, TEntity> _useCase = useCase;

        protected abstract TUseCaseRequest MapUseCaseRequest(TRequest request, Guid id);

        [MapException(typeof(EntityToUpdateNotFoundException), StatusCodes.Status404NotFound)]
        public virtual async ValueTask<IActionResult> Update(string id, TRequest request)
        {
            await _useCase.Execute(MapUseCaseRequest(request, new Guid(id)));
            return NoContent();
        }
    }
}
