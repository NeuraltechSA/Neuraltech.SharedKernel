using FluentValidation;
using Microsoft.Extensions.Hosting;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace Neuraltech.SharedKernel.Infraestructure.Extensions
{
    public static class ValidationExtensions
    {
        public static IHostApplicationBuilder UseFluentValidation<T>(
           this IHostApplicationBuilder builder
        )
        {
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<T>();

            return builder;
        }
    }
}
