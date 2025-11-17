using Neuraltech.SharedKernel.Domain.Base;

namespace Neuraltech.SharedKernel.Application.UseCases.Paginate
{
    public record PaginateResultDTO<TEntity>
        where TEntity : Entity
    {
        public required long Count { get; init; }
        public required long Page { get; init; }
        public required long PageSize { get; init; }
        public required IEnumerable<TEntity> Items { get; init; }
    }
}
