using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Application.UseCases.Base;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Base.Criteria;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Domain.Exceptions;
using Neuraltech.SharedKernel.Domain.Services;


namespace Neuraltech.SharedKernel.Application.UseCases.Paginate
{
    public abstract class PaginateUseCase<TEntity, TCriteria>(
        ILogger logger,
        IPaginateRepository<TEntity, TCriteria> repository
    ) : PaginateUseCase<BaseCriteria<TCriteria>, TEntity, TCriteria >(logger, repository)
        where TEntity : AggregateRoot
        where TCriteria : BaseCriteria<TCriteria>
    {
        protected override TCriteria MapCriteria(BaseCriteria<TCriteria> request)
        {
            return (TCriteria)request;
        }
         
    }

    public abstract class PaginateUseCase<TRequest, TEntity, TCriteria>(
        ILogger logger,
        IPaginateRepository<TEntity, TCriteria> repository
    ) : BaseUseCase<TRequest, PaginateResultDTO<TEntity>>(logger)
        where TEntity : AggregateRoot
        where TCriteria : BaseCriteria<TCriteria>
    {
        protected long DefaultPage = 1;
        protected long DefaultPageSize = 20;
        protected long MaxPageSize = 100;

        private readonly IPaginateRepository<TEntity, TCriteria> _repository = repository;
        protected abstract TCriteria MapCriteria(TRequest request);
        protected override async ValueTask<UseCaseResponse<PaginateResultDTO<TEntity>>> ExecuteLogic(TRequest request)
        {
            var criteria = MapCriteria(request);

            EnsureMaxPageSize(criteria);

            var items = await _repository.Find(criteria);
            var count = await _repository.Count(criteria);

            _logger.LogDebug("Found {Count} items for page {Page} with page size {PageSize}", count, criteria.GetPageNumber(), criteria.GetPageSize());

            return UseCaseResponse<PaginateResultDTO<TEntity>>.FromResult(new PaginateResultDTO<TEntity>
            {
                Items = items,
                Count = count,
                Page = criteria.GetPageNumber() ?? DefaultPage,
                PageSize = criteria.GetPageSize() ?? DefaultPageSize
            });
        }

        private void EnsureMaxPageSize(BaseCriteria<TCriteria> criteria)
        {
            if (criteria.HasPagination)
            {
                Ensure.InRange((long)criteria.GetPageSize()!, 0, MaxPageSize);
            }

        }

    }
}
