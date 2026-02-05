using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;

namespace Neuraltech.SharedKernel.Infraestructure.Extensions
{
    public static class LocalizationExtensions
    {
        public static IHostApplicationBuilder UseLocalization(
            this IHostApplicationBuilder builder,
            List<string> cultures,
            string defaultCulture
        )
        {
            builder.Services.AddLocalization();
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                List<CultureInfo> supportedCultures = [..cultures.Select(c => new CultureInfo(c))];
                options.DefaultRequestCulture = new RequestCulture(defaultCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            return builder;
        }
    }
}
