namespace Neuraltech.SharedKernel.Domain.Contracts
{
    public interface IProjectable<T>
        where T : IEntitySnapshot
    {
        T ToSnapshot();
    }
}
