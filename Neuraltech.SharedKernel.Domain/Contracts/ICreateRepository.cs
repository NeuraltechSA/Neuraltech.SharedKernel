using Neuraltech.SharedKernel.Domain.Base;

namespace Neuraltech.SharedKernel.Domain.Contracts
{
    public interface ICreateRepository<TEntity> where TEntity : Entity
    {
        ValueTask Create(TEntity entity);
    }
}
