using Neuraltech.SharedKernel.Domain.Base;

namespace Neuraltech.SharedKernel.Domain.Contracts
{
    public interface IEventBus
    {
        ValueTask Publish<TEvent>(TEvent @event) where TEvent : BaseEvent;
        ValueTask Publish<TEvent>(IEnumerable<TEvent> events) where TEvent : BaseEvent;
    }

}
