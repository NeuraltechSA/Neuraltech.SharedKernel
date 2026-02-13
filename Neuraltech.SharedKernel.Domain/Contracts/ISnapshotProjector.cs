
using Neuraltech.SharedKernel.Domain.Base;

namespace Neuraltech.SharedKernel.Domain.Contracts
{
    public interface ISnapshotProjector<TSnapshot>
        where TSnapshot : Snapshot

    {
        ValueTask ApplyProjection(TSnapshot snapshot);
    }
}
