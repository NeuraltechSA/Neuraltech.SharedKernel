using Neuraltech.SharedKernel.Domain.Base;

namespace Neuraltech.SharedKernel.Domain.Contracts
{
    public interface IPaginateRepository<TEntity, TCriteria>
        where TEntity : Entity
        where TCriteria : class
    {
        ValueTask<IEnumerable<TEntity>> Find(TCriteria criteria);
        ValueTask<long> Count(TCriteria criteria);
    }
}
