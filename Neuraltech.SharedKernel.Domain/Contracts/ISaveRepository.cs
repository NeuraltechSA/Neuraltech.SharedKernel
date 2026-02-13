using System.Threading.Tasks;
using Neuraltech.SharedKernel.Domain.Base;

namespace Neuraltech.SharedKernel.Domain.Contracts;

public interface ISaveRepository<TEntity>
    where TEntity : Entity
{
    ValueTask Save(TEntity entity);
}
