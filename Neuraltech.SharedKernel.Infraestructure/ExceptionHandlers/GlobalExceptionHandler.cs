using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Infraestructure.Localization;

namespace Neuraltech.SharedKernel.Infraestructure.ExceptionHandlers
{
    public sealed class GlobalExceptionHandler : BaseExceptionHandler
    {
        private readonly IStringLocalizer<SharedLocalization> _localizer;

        public GlobalExceptionHandler(
            IStringLocalizer<SharedLocalization> localizer,
            IProblemDetailsService problemDetailsService,
            ILogger<GlobalExceptionHandler> logger,
            IStringLocalizerFactory localizerFactory
        )
            : base(problemDetailsService, logger, localizerFactory)
        {
            _localizer = localizer;
        }

        protected override Exception? ParseException(Exception exception)
        {
            return exception;
        }

        protected override string ProblemDetail(
            Exception exception,
            HttpContext context,
            IStringLocalizer localizer
        )
        {
            return localizer["Error_UnhandledException_Detail"];
        }

        protected override string ProblemTitle(
            Exception exeception,
            HttpContext context,
            IStringLocalizer localizer
        )
        {
            return localizer["Error_UnhandledException_Title"];
        }

        protected override IStringLocalizer GetLocalizer(Exception exception, HttpContext context)
        {
            return _localizer;
        }

        protected override int StatusCode(Exception exception)
        {
            //if (exception is ApplicationException) return StatusCodes.Status400BadRequest;
            return StatusCodes.Status500InternalServerError;
        }
    }
}
