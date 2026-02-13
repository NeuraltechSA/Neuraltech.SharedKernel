using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Base.Criteria;

namespace Neuraltech.SharedKernel.Domain.Contracts;

public interface IRepository<TEntity, TCriteria>
    : ICreateRepository<TEntity>,
        IPaginateRepository<TEntity, TCriteria>,
        IFindByIdRepository<TEntity>,
        IUpdateRepository<TEntity>,
        IDeleteRepository<TEntity>,
        ISaveRepository<TEntity>
    where TEntity : Entity
    where TCriteria : IPaginable<TCriteria> { }
