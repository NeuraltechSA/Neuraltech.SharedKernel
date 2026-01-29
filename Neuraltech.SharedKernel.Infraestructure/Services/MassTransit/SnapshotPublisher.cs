using MassTransit;
using MassTransit.DependencyInjection;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Domain.Services;

namespace Neuraltech.SharedKernel.Infraestructure.Services.MassTransit
{
    public class SnapshotPublisher: ISnapshotPublisher
    {
        private readonly ITopicProducerProvider _producerProvider;
        private readonly ILogger<SnapshotPublisher> _logger;


        public SnapshotPublisher(
            ILogger<SnapshotPublisher> logger,
            ITopicProducerProvider producerProvider
        )
        {
            _logger = logger;
            _producerProvider = producerProvider;
        }

        public async ValueTask Publish<TSnapshot>(Guid id, TSnapshot? snapshot) 
            where TSnapshot : class, IEntitySnapshot
        {
            _logger.LogInformation("Publishing snapshot {0}", TSnapshot.SnapshotName);
            
            var producer = _producerProvider
                .GetProducer<Guid,TSnapshot>(
                    new Uri($"topic:{TSnapshot.SnapshotName}")
            );

            if(snapshot != null)
            {
                await producer.Produce(
                    id,
                    snapshot
                );
            }
            else
            {
                await producer.Produce(
                    id,
                    new { },
                    Pipe.Execute<KafkaSendContext<TSnapshot>>(context => {
                        context.ValueSerializer = new TombstoneSerializer<TSnapshot>();
                    })
                );
            }

            _logger.LogInformation("Snapshot {0} published", TSnapshot.SnapshotName);
        }
    }
}
