using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neuraltech.SharedKernel.Domain.Contracts;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Neuraltech.SharedKernel.Infraestructure.Extensions
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection UseInMemoryDb<T>(
        this IServiceCollection services)
            where T : DbContext, IUnitOfWork
        {
            services.AddDbContext<T>(options =>
                /*options.UseNpgsql(
                    builder.Configuration.GetConnectionString("postgres")
                )*/
                options.UseInMemoryDatabase("db")
            );
            services.AddScoped<IUnitOfWork>(c => c.GetRequiredService<T>());

            return services;
        }

        public static IHostApplicationBuilder
        UsePostgresDb<T>(
            this IHostApplicationBuilder builder, 
            string connectionStringName = "postgres",
            Action<NpgsqlDbContextOptionsBuilder>? pgOptions = null

        )
            where T : DbContext, IUnitOfWork
        {
            builder.Services.AddDbContext<T>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString(connectionStringName),
                    pgOptions
                )
            );
            builder.Services.AddScoped<IUnitOfWork>(c => c.GetRequiredService<T>());

            return builder;
        }
    }
}
