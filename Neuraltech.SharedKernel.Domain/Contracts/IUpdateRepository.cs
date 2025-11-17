using Neuraltech.SharedKernel.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace Neuraltech.SharedKernel.Domain.Contracts
{
    public interface IUpdateRepository<TEntity> where TEntity : Entity
    {
        ValueTask Update(TEntity entity);
    }
}
