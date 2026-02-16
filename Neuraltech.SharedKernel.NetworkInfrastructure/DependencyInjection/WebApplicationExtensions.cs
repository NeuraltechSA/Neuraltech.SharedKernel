using Microsoft.AspNetCore.Builder;

namespace Neuraltech.SharedKernel.NetworkInfrastructure.DependencyInjection
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseDefaultConfiguration(this WebApplication app)
        {
            app.UseAuthorization();
            app.MapControllers();
            app.MapReverseProxy();
            return app;
        }
    }
}
