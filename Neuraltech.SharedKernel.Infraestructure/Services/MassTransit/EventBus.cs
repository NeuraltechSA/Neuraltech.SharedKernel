using Microsoft.Extensions.Logging;
using MassTransit;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Domain.Base;

namespace Neuraltech.SharedKernel.Infraestructure.Services.MassTransit
{
    public class EventBus : IEventBus
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<EventBus> _logger;

        public EventBus(IPublishEndpoint publishEndpoint, ILogger<EventBus> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async ValueTask Publish<TEvent>(TEvent @event) where TEvent : BaseEvent
        {
            await Publish(new List<TEvent> { @event });
        }
        public async ValueTask Publish<TEvent>(IEnumerable<TEvent> events) where TEvent : BaseEvent
        {
            foreach (var @event in events)
            {
                _logger.LogInformation("Publishing event: {Event}", @event.GetType().Name);
                await _publishEndpoint.Publish(@event, @event.GetType(), context =>
                {
                    context.SetRoutingKey(@event.MessageName);
                });
                _logger.LogInformation("Event published: {Event}", @event.GetType().Name);
            }
        }

        
    }
}
