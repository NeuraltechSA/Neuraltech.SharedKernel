using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Infraestructure.Exceptions;
using Neuraltech.SharedKernel.Infraestructure.Localization;

namespace Neuraltech.SharedKernel.Infraestructure.ExceptionHandlers
{
    public sealed class IdAlreadyInDbExceptionHandler : BaseExceptionHandler<IdAlreadyInDbException>
    {
      
        private readonly IStringLocalizer<SharedLocalization> _localizer;
        public IdAlreadyInDbExceptionHandler(
            IProblemDetailsService problemDetailsService, 
            ILogger<GlobalExceptionHandler> logger,
            IStringLocalizer<SharedLocalization> localizer
        ) : base(problemDetailsService, logger)
        {
            _localizer = localizer;
        }

        protected override IdAlreadyInDbException? ParseException(Exception exception)
        {
            if (exception is not IdAlreadyInDbException dbException) return null;
            return dbException;
        }

        protected override string ProblemDetail(IdAlreadyInDbException exception, HttpContext context)
        {
            return _localizer.GetString("Error_IdAlreadyInDb_Detail", exception.DuplicatedId);
        }

        protected override string ProblemTitle(IdAlreadyInDbException exeception, HttpContext context)
        {
            return _localizer["Error_IdAlreadyInDb_Title"];
        }

        protected override int StatusCode(IdAlreadyInDbException exception)
        {
            return StatusCodes.Status409Conflict;
        }
    }
}
