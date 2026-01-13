using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Neuraltech.SharedKernel.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neuraltech.SharedKernel.Infraestructure.Extensions
{
    public static class TimeExtensions
    {
        public static IServiceCollection UseTimeProvider(
            this IServiceCollection services
        )
        {
            services.AddSingleton(TimeProvider.System);
            return services;
        }
    }
}
