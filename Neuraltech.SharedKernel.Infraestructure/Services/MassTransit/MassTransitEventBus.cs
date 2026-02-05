using Microsoft.Extensions.Logging;
using MassTransit;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Domain.Base;
using MassTransit.DependencyInjection;

namespace Neuraltech.SharedKernel.Infraestructure.Services.MassTransit
{
    public class MassTransitEventBus : IEventBus
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ITopicProducerProvider _producerProvider;
        //private readonly Bind<IKafkaBus, IPublishEndpoint> _kafkaPublishEndpoint;
        private readonly ILogger<MassTransitEventBus> _logger;

        public MassTransitEventBus(
            IPublishEndpoint publishEndpoint,
            ITopicProducerProvider producerProvider,
            //Bind<IKafkaBus, IPublishEndpoint> kafkaPublishEndpoint,
            ILogger<MassTransitEventBus> logger
        )
        {
            _publishEndpoint = publishEndpoint;
            _producerProvider = producerProvider;
            //_kafkaPublishEndpoint = kafkaPublishEndpoint;
            _logger = logger;
        }

        public async ValueTask Publish<TEvent>(TEvent @event) where TEvent :  BaseEvent
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

            await _producerProvider.GetProducer<TEvent>(new Uri($"topic:{@event.MessageName}"))
                .Produce(@event);

            /*await _kafkaPublishEndpoint.Value.Publish(@event, @event.GetType(), context =>
            {
                context.SetRoutingKey(@event.MessageName);
            });*/
            _logger.LogInformation("Event published to Kafka: {Event}", @event.GetType().Name);
        }

        private async ValueTask PublishToMemory<TEvent>(TEvent @event) where TEvent : BaseEvent
        {
            _logger.LogInformation("Publishing event to In-Memory: {Event}", @event.GetType().Name);
            await _publishEndpoint.Publish(@event, @event.GetType());
            _logger.LogInformation("Event published to In-Memory: {Event}", @event.GetType().Name);
        }


    }
}
