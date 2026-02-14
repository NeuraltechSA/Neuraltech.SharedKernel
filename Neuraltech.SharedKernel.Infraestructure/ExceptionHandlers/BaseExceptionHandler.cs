using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Neuraltech.SharedKernel.Infraestructure.ExceptionHandlers
{
    public abstract class BaseExceptionHandler : BaseExceptionHandler<Exception>
    {
        protected BaseExceptionHandler(
            IProblemDetailsService problemDetailsService,
            ILogger<GlobalExceptionHandler> logger,
            IStringLocalizerFactory localizerFactory
        )
            : base(problemDetailsService, logger, localizerFactory) { }
    }

    public abstract class BaseExceptionHandler<TException> : IExceptionHandler
        where TException : Exception
    {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IStringLocalizerFactory _localizerFactory;

        public BaseExceptionHandler(
            IProblemDetailsService problemDetailsService,
            ILogger<GlobalExceptionHandler> logger,
            IStringLocalizerFactory localizerFactory
        )
        {
            _problemDetailsService = problemDetailsService;
            _logger = logger;
            _localizerFactory = localizerFactory;
        }

        protected abstract TException? ParseException(Exception exception);
        protected abstract int StatusCode(TException exception);
        protected abstract string ProblemTitle(
            TException exeception,
            HttpContext context,
            IStringLocalizer localizer
        );
        protected abstract string ProblemDetail(
            TException exception,
            HttpContext context,
            IStringLocalizer localizer
        );

        /// <summary>
        /// <see cref="Filters.ControllerTypeFilter"/>
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual IStringLocalizer GetLocalizer(TException exception, HttpContext context)
        {
            var controllerType = context.Items["ControllerType"] as Type;
            if (controllerType is null)
                return _localizerFactory.Create(typeof(Localization.SharedLocalization));
            return _localizerFactory.Create(controllerType);
        }

        protected virtual Dictionary<string, object?> GetExtensions(TException exception, HttpContext context) => [];

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            var parsedException = ParseException(exception);
            if (parsedException is null)
                return false;

            var localizer = GetLocalizer(parsedException, httpContext);
            var detail = ProblemDetail(parsedException, httpContext, localizer);
            var title = ProblemTitle(parsedException, httpContext, localizer);

            _logger.LogError(parsedException, "{Message}", parsedException.Message);

            httpContext.Response.StatusCode = StatusCode(parsedException);


            return await _problemDetailsService.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    Exception = parsedException,
                    ProblemDetails = new ProblemDetails
                    {
                        //Type = parsedException.GetType().Name,
                        Title = title,
                        Detail = detail,
                        Extensions = GetExtensions(parsedException, httpContext)

                    },
                }
            );
        }
    }
}
