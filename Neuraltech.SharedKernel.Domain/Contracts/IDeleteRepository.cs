using Neuraltech.SharedKernel.Domain.Base;

namespace Neuraltech.SharedKernel.Domain.Contracts
{
    public interface IDeleteRepository<TEntity> where TEntity : Entity
    {
        ValueTask Delete(TEntity entity);
    }
}
