using Confluent.Kafka;

namespace Neuraltech.SharedKernel.Infraestructure.Services.MassTransit
{
    class TombstoneSerializer<T> :
        IAsyncSerializer<T>
    {
        public Task<byte[]> SerializeAsync(T data, SerializationContext context)
        {
            return Task.FromResult(Array.Empty<byte>());
        }
    }

}
