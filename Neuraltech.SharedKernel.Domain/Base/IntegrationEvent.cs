using Neuraltech.SharedKernel.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neuraltech.SharedKernel.Domain.Base
{
    public abstract class IntegrationEvent : BaseEvent
    {
        public abstract string MessageName { get; }
    }
}
