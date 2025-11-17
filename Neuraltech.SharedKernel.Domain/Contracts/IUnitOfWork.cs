
namespace Neuraltech.SharedKernel.Domain.Contracts
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
