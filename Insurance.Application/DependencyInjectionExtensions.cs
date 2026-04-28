using Insurance.Application.Insuring.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Insurance.Application
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddMediatR(this IServiceCollection services)
        {
            services.AddMediatR(config => 
            {
                config.RegisterServicesFromAssembly(typeof(ConfirmBookingCommand).Assembly);
            });

            return services;
        }
    }
}