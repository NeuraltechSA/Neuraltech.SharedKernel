using Microsoft.AspNetCore.Mvc;
using Neuraltech.SharedKernel.Application.UseCases.Paginate;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Base.Criteria;
using Neuraltech.SharedKernel.Infraestructure.DTO;


namespace Neuraltech.SharedKernel.Infraestructure.Handlers
{
    public abstract class PaginateHandler<TEntity, TCriteria, TRequest, TResponseEntity>(
            PaginateUseCase<TEntity, TCriteria> useCase
        ) : ControllerBase
        where TRequest : PaginateRequestDTO
        where TCriteria : BaseCriteria<TCriteria>
        where TEntity : AggregateRoot
    {
        private readonly PaginateUseCase<TEntity, TCriteria> _useCase = useCase;
        protected abstract TResponseEntity MapResponseEntity(TEntity entity);
        protected virtual TCriteria MapCriteria(TRequest request)
        {
            return BaseCriteria<TCriteria>
                .Create()
                .Paginate(request.Page, request.Limit);
        }

        public virtual async ValueTask<IActionResult> Paginate([FromQuery] TRequest request)
        {
            var criteria = MapCriteria(request);
            var result = await _useCase.Execute(criteria);
            var response = new PaginateResponseDTO<TResponseEntity>
            {
                Count = result.Value!.Count,
                Page = result.Value.Page,
                PageSize = result.Value.PageSize,
                Items = result.Value.Items.Select(MapResponseEntity)
            };
            return Ok(response);
        }
    }
}
