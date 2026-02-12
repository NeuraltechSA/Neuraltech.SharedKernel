using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Application.UseCases.Delete;
using Neuraltech.SharedKernel.Domain.Base;

namespace Neuraltech.SharedKernel.Infraestructure.Handlers
{
    public abstract class DeleteHandler<TEntity>(DeleteUseCase<TEntity> useCase, ILogger logger)
        : ControllerBase
        where TEntity : AggregateRoot
    {
        private readonly DeleteUseCase<TEntity> _useCase = useCase;
        private readonly ILogger _logger = logger;

        // Idempotent
        public virtual async ValueTask<IActionResult> Delete(string id)
        {
            _logger.LogInformation(
                "Received request to delete entity of type {EntityType} with id {Id}",
                typeof(TEntity).Name,
                id
            );
            await _useCase.Execute(new Guid(id));
            return NoContent();
        }
    }
}
