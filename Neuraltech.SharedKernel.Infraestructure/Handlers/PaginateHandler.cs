using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Neuraltech.SharedKernel.Application.UseCases.Paginate;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Base.Criteria;

//using Neuraltech.SharedKernel.Infraestructure.DTO;

namespace Neuraltech.SharedKernel.Infraestructure.Handlers
{
    public abstract class PaginateHandler<TEntity, TCriteria, TRequest, TResponseEntity>(
        PaginateUseCase<TEntity, TCriteria> useCase,
        IValidator<TRequest> validator
    ) : ControllerBase
        where TRequest : PaginateRequestDTO
        where TCriteria : BaseCriteria<TCriteria>
        where TEntity : AggregateRoot
    {
        private readonly PaginateUseCase<TEntity, TCriteria> _useCase = useCase;
        private readonly IValidator<TRequest> _validator = validator;

        protected abstract TResponseEntity MapResponseEntity(TEntity entity);
        private TCriteria CreatePaginatedCriteria(TRequest request)
        {
            var criteria = BaseCriteria<TCriteria>
                .Create()
                .Paginate(
                    request.Page, 
                    request.Limit
                );
            return criteria;
        }
        
        protected abstract TCriteria MapCriteria(TRequest request, TCriteria baseCriteria);

        public virtual async ValueTask<IActionResult> Paginate([FromQuery] TRequest request)
        {
            await _validator.ValidateAndThrowAsync(request);

            var criteria = MapCriteria(request, CreatePaginatedCriteria(request));

            var result = await _useCase.Execute(criteria);
            var response = new PaginateResponseDTO<TResponseEntity>
            {
                Count = result.Value!.Count,
                Page = result.Value.Page,
                PageSize = result.Value.PageSize,
                Items = result.Value.Items.Select(MapResponseEntity),
            };
            return Ok(response);
        }
    }
}
