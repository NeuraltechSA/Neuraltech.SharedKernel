using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Domain.Exceptions;

namespace Neuraltech.SharedKernel.Infraestructure.ExceptionHandlers
{
    public sealed class EntityToDeleteNotFoundExceptionHandler
        : BaseExceptionHandler<EntityToDeleteNotFoundException>
    {
        public EntityToDeleteNotFoundExceptionHandler(
            IStringLocalizerFactory localizerFactory,
            IProblemDetailsService problemDetailsService,
            ILogger<GlobalExceptionHandler> logger
        )
            : base(problemDetailsService, logger, localizerFactory) { }

        protected override EntityToDeleteNotFoundException? ParseException(Exception exception)
        {
            if (exception is not EntityToDeleteNotFoundException notFoundException)
                return null;
            return notFoundException;
        }

        protected override string ProblemDetail(
            EntityToDeleteNotFoundException exception,
            HttpContext context,
            IStringLocalizer localizer
        )
        {
            return localizer.GetString("Error_EntityToDeleteNotFound_Detail", exception.EntityId);
        }

        protected override string ProblemTitle(
            EntityToDeleteNotFoundException exception,
            HttpContext context,
            IStringLocalizer localizer
        )
        {
            return localizer.GetString("Error_EntityToDeleteNotFound_Title", exception.EntityId);
        }

        protected override int StatusCode(EntityToDeleteNotFoundException exception)
        {
            return StatusCodes.Status204NoContent;
        }
    }
}
