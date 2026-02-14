using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Infraestructure.Localization;

namespace Neuraltech.SharedKernel.Infraestructure.ExceptionHandlers
{
    public class ValidationExceptionHandler : BaseExceptionHandler<ValidationException>
    {
        private readonly IStringLocalizer<SharedLocalization> _localizer;

        public ValidationExceptionHandler(
            IProblemDetailsService problemDetailsService,
            ILogger<GlobalExceptionHandler> logger,
            IStringLocalizerFactory localizerFactory,
            IStringLocalizer<SharedLocalization> localizer
        )
            : base(problemDetailsService, logger, localizerFactory)
        {
            _localizer = localizer;
        }

        protected override ValidationException? ParseException(Exception exception)
        {
            if (exception is ValidationException validationException)
                return validationException;
            return null;
        }

        protected override string ProblemDetail(
            ValidationException exception,
            HttpContext context,
            IStringLocalizer localizer
        )
        {
            return _localizer["Error_Validation_Detail"];
        }

        protected override string ProblemTitle(
            ValidationException exeception,
            HttpContext context,
            IStringLocalizer localizer
        )
        {
            return _localizer["Error_Validation_Title"];
        }

        protected override Dictionary<string, object?> GetExtensions(
            ValidationException exception,
            HttpContext context
        )
        {
            var errors = exception
                .Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key.ToLowerInvariant(),
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            return new() { { "errors", errors } };
        }

        protected override int StatusCode(ValidationException exception)
        {
            return StatusCodes.Status400BadRequest;
        }
    }
}
