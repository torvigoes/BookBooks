using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Mapster;

namespace BookBooks.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // 1. Add MediatR
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            // Validation Pipeline behavior could be injected here
        });

        // 2. Add FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // 3. Add Mapster Config (globally)
        TypeAdapterConfig.GlobalSettings.Scan(assembly);

        return services;
    }
}
