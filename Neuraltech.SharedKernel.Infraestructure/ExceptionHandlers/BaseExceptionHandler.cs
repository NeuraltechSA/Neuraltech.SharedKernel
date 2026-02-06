using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Neuraltech.SharedKernel.Infraestructure.ExceptionHandlers
{
    public abstract class BaseExceptionHandler : BaseExceptionHandler<Exception>
    {
        protected BaseExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger) : base(problemDetailsService, logger)
        {
        }
    }

    public abstract class BaseExceptionHandler<TException> : IExceptionHandler
        where TException : Exception
    {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public BaseExceptionHandler(
            IProblemDetailsService problemDetailsService,
            ILogger<GlobalExceptionHandler> logger
        )
        {
            _problemDetailsService = problemDetailsService;
            _logger = logger;
        }

        protected abstract TException? ParseException(Exception exception);
        protected abstract int StatusCode(TException exception);
        protected abstract string ProblemTitle(TException exeception, HttpContext context);
        protected abstract string ProblemDetail(TException exception, HttpContext context);

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var parsedException = ParseException(exception);
            if (parsedException is null) return false;

            _logger.LogError(exception, exception.Message);

            httpContext.Response.StatusCode = StatusCode(parsedException);

            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = parsedException,
                ProblemDetails = new ProblemDetails
                {
                    //Type = parsedException.GetType().Name,
                    Title = ProblemTitle(parsedException, httpContext),
                    Detail = ProblemDetail(parsedException, httpContext)
                }
            });
        }
    }
}
