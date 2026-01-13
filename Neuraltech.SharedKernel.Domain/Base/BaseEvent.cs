namespace Neuraltech.SharedKernel.Domain.Base
{
    /// <summary>
    /// Clase base para eventos de dominio locales.
    /// Estos eventos no requieren MessageName ya que no cruzan límites de bounded context.
    /// </summary>
    public abstract record BaseEvent
    {
    }
}
