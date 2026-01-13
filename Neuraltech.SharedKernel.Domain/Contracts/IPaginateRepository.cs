using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Base.Criteria;

namespace Neuraltech.SharedKernel.Domain.Contracts
{
    public interface IPaginateRepository<TEntity, TCriteria>
        where TEntity : Entity
        where TCriteria : IPaginable<TCriteria>
    {
        ValueTask<IEnumerable<TEntity>> Find(TCriteria criteria);
        ValueTask<long> Count(TCriteria criteria);
    }
}
