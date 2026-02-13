using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Contracts;
using Wolverine;

namespace Neuraltech.SharedKernel.Infraestructure.Services.WolverineFX
{
    public class WolverineSnapshotPublisher : ISnapshotPublisher
    {
        private readonly IMessageBus _bus;
        

        public WolverineSnapshotPublisher(IMessageBus bus)
        {
            _bus = bus;
        }

        public async ValueTask Publish<TSnapshot>(Guid id, TSnapshot? snapshot) 
            where TSnapshot : Snapshot
        {
            
            await _bus.PublishAsync(snapshot, new DeliveryOptions
            {
                PartitionKey = id.ToString()
            });
        }
    }
}
