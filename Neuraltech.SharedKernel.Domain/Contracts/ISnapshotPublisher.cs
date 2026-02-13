using Neuraltech.SharedKernel.Domain.Base;

namespace Neuraltech.SharedKernel.Domain.Contracts
{
    public interface ISnapshotPublisher
    {
        ValueTask Publish<TSnapshot>(Guid id, TSnapshot? snapshot)
            where TSnapshot : Snapshot;
    }
}
