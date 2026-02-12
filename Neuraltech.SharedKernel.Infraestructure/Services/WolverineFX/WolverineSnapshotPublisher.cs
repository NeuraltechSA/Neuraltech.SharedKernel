using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Contracts;
using Wolverine;
using Wolverine.Runtime;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            where TSnapshot : class
        {
            //GeminiReference.Backoffice.Posts.PostSnapshot

            if(snapshot == null)
            {
                var endpoint =  _bus
                    .EndpointFor("GeminiReference.Backoffice.Posts.PostSnapshot");

                MessageBus a = (MessageBus)_bus;

                //a.Runtime.Options.Ser


                var envelope = new Envelope
                {
                    Data = Array.Empty<byte>(),
                    TopicName = "GeminiReference.Backoffice.Posts.PostSnapshot"
            //GeminiReference.Backoffice.Posts.PostSnapshot
                    //Sender = a.Runtime.Endpoints.GetOrBuildSendingAgent(endpoint.Uri)
                };
                



                /*
                await _bus
                    .EndpointFor("GeminiReference.Backoffice.Posts.PostSnapshot")
                    .SendRawMessageAsync(Array.Empty<byte>());*/
            }


            await _bus.PublishAsync(snapshot, new DeliveryOptions
            {
                PartitionKey = id.ToString()
            });
        }
    }
}
