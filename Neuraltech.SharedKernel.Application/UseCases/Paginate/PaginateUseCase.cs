using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Application.UseCases.Base;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Base.Criteria;
using Neuraltech.SharedKernel.Domain.Contracts;


namespace Neuraltech.SharedKernel.Application.UseCases.Paginate
{
    public abstract class PaginateUseCase<TEntity, TCriteria>(
        ILogger logger,
        IPaginateRepository<TEntity, TCriteria> repository
    ) : PaginateUseCase<TEntity, TCriteria, TCriteria>(logger, repository)
        where TEntity : AggregateRoot
        where TCriteria : IPaginable<TCriteria>
    {
        protected override TCriteria MapCriteria(TCriteria request)
        {
            return request;
        }
    }

    public abstract class PaginateUseCase<TEntity, TCriteria, TRequest>(
        ILogger logger,
        IPaginateRepository<TEntity, TCriteria> repository
    ) : BaseUseCase<TRequest, PaginateResultDTO<TEntity>>(logger)
        where TCriteria : IPaginable<TCriteria>
        where TEntity : AggregateRoot
    {
        protected long DefaultPage = 1;
        protected long DefaultPageSize = 20;
        protected long MaxPageSize = 100;

        private readonly IPaginateRepository<TEntity, TCriteria> _repository = repository;
        protected abstract TCriteria MapCriteria(TRequest request);

        protected override async ValueTask<UseCaseResponse<PaginateResultDTO<TEntity>>> 
            ExecuteLogic(TRequest request)
        {
            var criteria = MapCriteria(request);

            var items = await _repository.Find(criteria);
            var count = await _repository.Count(criteria);

            _logger.LogDebug("Found {Count} items for page {Page} with page size {PageSize}", count, criteria.GetPageNumber(), criteria.GetPageSize());

            return UseCaseResponse<PaginateResultDTO<TEntity>>.FromResult(new PaginateResultDTO<TEntity>
            {
                Items = items,
                Count = count,
                Page = (long)criteria.GetPageNumber()!,
                PageSize = (long)criteria.GetPageSize()!
            });
        }
    }
}
