namespace Neuraltech.SharedKernel.Domain.Base
{
    /// <summary>
    /// Clase base para eventos de integración que cruzan límites de bounded contexts.
    /// Estos eventos se publican a través de RabbitMQ y requieren un MessageName para routing.
    /// </summary>
    public abstract record IntegrationEvent : BaseEvent
    {
        /// <summary>
        /// Nombre del mensaje utilizado para routing en RabbitMQ.
        /// </summary>
        public abstract string MessageName { get; }
    }
}
