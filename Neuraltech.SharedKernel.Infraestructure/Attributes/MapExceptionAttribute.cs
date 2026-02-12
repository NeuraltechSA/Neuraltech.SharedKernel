using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Neuraltech.SharedKernel.Infraestructure.Attributes
{
    [Obsolete("Consider using BaseExceptionHandler")]
    public class MapExceptionAttribute : ExceptionFilterAttribute
    {
        public Type ExceptionType { get; }
        public int StatusCode { get; }
        public string? CustomMessage { get; }

        public MapExceptionAttribute(Type exceptionType, int statusCode, string? customMessage = null)
        {
            if (!typeof(Exception).IsAssignableFrom(exceptionType))
                throw new ArgumentException("Type should be an exception", nameof(exceptionType));

            ExceptionType = exceptionType;
            StatusCode = statusCode;
            CustomMessage = customMessage;
        }


        public override void OnException(ExceptionContext context)
        {
            var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<MapExceptionAttribute>();

            logger.LogError(context.Exception, $"Excepcion on request");

            if(context.Exception.GetType() == ExceptionType)
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCode,
                    Content = CustomMessage ?? context.Exception.Message
                };
                context.ExceptionHandled = true;
                return;
            }


            base.OnException(context);
        }
    }
}
