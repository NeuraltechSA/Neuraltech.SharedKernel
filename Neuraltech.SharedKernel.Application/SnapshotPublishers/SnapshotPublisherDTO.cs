using Neuraltech.SharedKernel.Domain.Base;

namespace Neuraltech.SharedKernel.Application.SnapshotPublishers
{
    public record SnapshotPublisherDTO<TEntity>
                where TEntity : AggregateRoot

    {
        public required TEntity Entity { get; init; }
        public required bool MarkAsDeleted { get; init; }
    }
}
