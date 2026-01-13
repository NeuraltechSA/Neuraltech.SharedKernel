using Microsoft.Extensions.Logging;
using MassTransit;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Domain.Base;
using MassTransit.DependencyInjection;

namespace Neuraltech.SharedKernel.Infraestructure.Services.MassTransit
{
    public class EventBus : IEventBus
    {
        private readonly IPublishEndpoint _memoryPublishEndpoint;
        private readonly Bind<IKafkaBus, IPublishEndpoint> _kafkaPublishEndpoint;
        private readonly ILogger<EventBus> _logger;

        public EventBus(
            IPublishEndpoint memoryPublishEndpoint,
            Bind<IKafkaBus, IPublishEndpoint> kafkaPublishEndpoint,
            ILogger<EventBus> logger
        )
        {
            _memoryPublishEndpoint = memoryPublishEndpoint;
            _kafkaPublishEndpoint = kafkaPublishEndpoint;
            _logger = logger;
        }

        public async ValueTask Publish<TEvent>(TEvent @event) where TEvent : BaseEvent
        {
            await Publish(new List<TEvent> { @event });
        }
        public async ValueTask Publish<TEvent>(IEnumerable<TEvent> events) where TEvent : BaseEvent
        {
            foreach(var @event in events)
            {
                if (@event is IntegrationEvent integrationEvent)
                {
                    await PublishToKafka(integrationEvent);
                }
                else
                {
                    await PublishToMemory(@event);
                }
            }
        }
        private async ValueTask PublishToKafka<TEvent>(TEvent @event) where TEvent : IntegrationEvent
        {
            _logger.LogInformation("Publishing event to Kafka: {Event}", @event.GetType().Name);
            await _kafkaPublishEndpoint.Value.Publish(@event, @event.GetType(), context =>
            {
                context.SetRoutingKey(@event.MessageName);
            });
            _logger.LogInformation("Event published to Kafka: {Event}", @event.GetType().Name);
        }

        private async ValueTask PublishToMemory<TEvent>(TEvent @event) where TEvent : BaseEvent
        {
            _logger.LogInformation("Publishing event to In-Memory: {Event}", @event.GetType().Name);
            await _memoryPublishEndpoint.Publish(@event, @event.GetType());
            _logger.LogInformation("Event published to In-Memory: {Event}", @event.GetType().Name);
        }


    }
}
