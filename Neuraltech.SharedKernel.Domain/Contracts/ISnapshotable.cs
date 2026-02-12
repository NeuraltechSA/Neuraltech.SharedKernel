namespace Neuraltech.SharedKernel.Domain.Contracts
{
    public interface ISnapshotable<TEntity, TSnapshot>
    {
        TSnapshot ToSnapshot();
        static abstract TEntity FromSnapshot(TSnapshot snapshot);
    }
}
