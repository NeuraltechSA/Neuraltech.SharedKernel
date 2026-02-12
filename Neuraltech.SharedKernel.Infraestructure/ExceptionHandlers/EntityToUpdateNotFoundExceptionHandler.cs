using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Domain.Exceptions;

namespace Neuraltech.SharedKernel.Infraestructure.ExceptionHandlers
{
    public sealed class EntityToUpdateNotFoundExceptionHandler
        : BaseExceptionHandler<EntityToUpdateNotFoundException>
    {
        public EntityToUpdateNotFoundExceptionHandler(
            IProblemDetailsService problemDetailsService,
            ILogger<GlobalExceptionHandler> logger,
            IStringLocalizerFactory localizerFactory
        )
            : base(problemDetailsService, logger, localizerFactory) { }

        protected override EntityToUpdateNotFoundException? ParseException(Exception exception)
        {
            if (exception is not EntityToUpdateNotFoundException notFoundException)
                return null;
            return notFoundException;
        }

        protected override string ProblemDetail(
            EntityToUpdateNotFoundException exception,
            HttpContext context,
            IStringLocalizer localizer
        )
        {
            return localizer.GetString("Error_EntityToUpdateNotFound_Detail");
        }

        protected override string ProblemTitle(
            EntityToUpdateNotFoundException exception,
            HttpContext context,
            IStringLocalizer localizer
        )
        {
            return localizer.GetString("Error_EntityToUpdateNotFound_Title");
        }

        protected override int StatusCode(EntityToUpdateNotFoundException exception)
        {
            return StatusCodes.Status404NotFound;
        }
    }
}
