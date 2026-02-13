namespace Neuraltech.SharedKernel.Domain.Base
{
    public abstract class Snapshot
    {
        public bool IsDeleted { get; set; }
        public abstract string SnapshotName { get; }
    }
}
