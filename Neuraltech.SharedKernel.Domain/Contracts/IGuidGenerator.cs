using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Neuraltech.SharedKernel.Domain.Contracts
{
    public interface IGuidGenerator
    {
        Guid GenerateGuid();
    }
}
