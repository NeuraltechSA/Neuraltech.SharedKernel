using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
            /// Changed to explicit validation in controllers
            //builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<T>();

            return builder;
        }

        public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
        {
            foreach (var error in result.Errors)
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }
    }
}
