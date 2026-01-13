namespace Neuraltech.SharedKernel.Domain.Base
{
    public abstract record Projection : IntegrationEvent
    {
        /// <summary>
        /// Timestamp de cuando se creó la proyección.
        /// </summary>
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
