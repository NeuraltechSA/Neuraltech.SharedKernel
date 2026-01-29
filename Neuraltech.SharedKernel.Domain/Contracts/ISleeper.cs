using System;
using System.Collections.Generic;
using System.Text;

namespace Neuraltech.SharedKernel.Domain.Contracts
{
    public interface ISleeper
    {
        Task Sleep(TimeSpan delay, CancellationToken cancellationToken = default);
    }
}
