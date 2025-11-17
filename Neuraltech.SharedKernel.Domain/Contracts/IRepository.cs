
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.ValueObjects;

namespace Neuraltech.SharedKernel.Domain.Contracts;

public interface IRepository<TEntity, TCriteria> : 
    ICreateRepository<TEntity>, 
    IPaginateRepository<TEntity, TCriteria>,
    IFindByIdRepository<TEntity>,
    IUpdateRepository<TEntity>,
    IDeleteRepository<TEntity>
    where TEntity : Entity
    where TCriteria : class
{
}