using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.ValueObjects;
using System.Security.Cryptography;

namespace Neuraltech.SharedKernel.Domain.Contracts
{
    public interface IFindByIdRepository<TEntity>
        where TEntity : Entity
    {

        ValueTask<TEntity?> Find(Guid id);
    }
}
