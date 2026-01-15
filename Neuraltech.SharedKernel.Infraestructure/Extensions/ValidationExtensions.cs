using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Neuraltech.SharedKernel.Domain.Contracts;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
