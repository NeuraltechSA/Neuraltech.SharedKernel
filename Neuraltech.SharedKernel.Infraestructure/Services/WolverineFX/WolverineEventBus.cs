using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Contracts;
using Wolverine;

namespace Neuraltech.SharedKernel.Infraestructure.Services.WolverineFX
{
    public class WolverineEventBus : IEventBus
    {
        private readonly IMessageBus _bus;
        public WolverineEventBus(IMessageBus bus)
        {
            _bus = bus;
        }

        public async ValueTask Publish<TEvent>(TEvent @event) where TEvent : BaseEvent
        {
            await _bus.PublishAsync(@event);
        }

        public async ValueTask Publish<TEvent>(IEnumerable<TEvent> events) where TEvent : BaseEvent
        {
            foreach (var @event in events)
            {
                await Publish(@event);
            }
        }
    }
}
