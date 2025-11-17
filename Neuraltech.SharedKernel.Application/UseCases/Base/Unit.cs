namespace Neuraltech.SharedKernel.Application.UseCases.Base
{
    /// <summary>
    /// Representa un tipo vacío utilizado como alternativa a void en contextos donde se requiere un tipo de retorno.
    /// Implementa el patrón Singleton para garantizar una única instancia en toda la aplicación.
    /// </summary>
    /// <remarks>
    /// Este tipo es útil en programación funcional y cuando se trabaja con tipos genéricos
    /// que requieren un tipo de retorno pero no hay datos significativos que devolver.
    /// Por ejemplo, en UseCases que realizan operaciones sin resultado específico pero
    /// necesitan mantener la consistencia del patrón UseCaseResponse&lt;TResult&gt;.
    /// </remarks>
    public sealed class Unit
    {
        /// <summary>
        /// Obtiene la única instancia de <see cref="Unit"/> disponible en la aplicación.
        /// </summary>
        /// <value>
        /// La instancia singleton de <see cref="Unit"/>.
        /// </value>
        /// <example>
        /// <code>
        /// return UseCaseResponse&lt;Unit&gt;.Success(Unit.Value);
        /// </code>
        /// </example>
        public static readonly Unit Value = new();

        /// <summary>
        /// Constructor privado que previene la creación de instancias adicionales desde fuera de la clase.
        /// </summary>
        private Unit() { }
    }

}
