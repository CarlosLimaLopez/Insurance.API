using Insurance.Application.Insuring.Messages;
using Insurance.Application.Interfaces;
using Insurance.Domain.Insuring.Interfaces;
using Insurance.Domain.Interfaces;
using Insurance.Infrastructure.ExternalServices;
using Insurance.Infrastructure.Persistence.Repositories;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Insurance.Infrastructure.Persistence;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration["FlexibleInsuredApi:Host"];
        if(host == null)
            throw new InvalidOperationException("FlexibleInsuredApi:Host configuration is missing.");

        services.AddHttpClient("FlexibleInsuredApi", c =>
        {
            c.BaseAddress = new Uri(host);
        });

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddMessaging(configuration);

        // Repositories
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IInsuredBookingRepository, InsuredBookingRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<InsuranceDbContext>());

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if(string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        
        services.AddDbContext<InsuranceDbContext>((sp, options) =>
            options
                .UseNpgsql(connectionString)
            );

        services.AddScoped<IReadDbContext>(sp => sp.GetRequiredService<InsuranceDbContext>());

        services.AddMassTransit(x =>
        {
            // NO registramos el consumidor todavía, porque la API no existe.
            //x.AddConsumer<IssueInsurancePolicyConsumer>();

            // Config transactional outbox for publishing messages when the transaction commits successfully
            x.AddEntityFrameworkOutbox<InsuranceDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });
        });

        return services;
    }

    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            // Uncomment when the API is ready
            //x.AddConsumer<IssueInsurancePolicyConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var host = configuration["RabbitMq:Host"] ?? "localhost";
                var virtualHost = configuration["RabbitMq:VirtualHost"] ?? "/";

                cfg.Publish<IssueInsurancePolicyMessage>(p =>
                {
                    p.Durable = true;
                });

                cfg.Host(host, virtualHost, h =>
                {
                    h.Username(configuration["RabbitMq:Username"] ?? "guest");
                    h.Password(configuration["RabbitMq:Password"] ?? "guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}