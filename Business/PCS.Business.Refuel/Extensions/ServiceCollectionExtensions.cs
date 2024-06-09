namespace PCS.Business.Refuel.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRefuel(this IServiceCollection services) {
        services.AddServices();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services) {
        var types = typeof(ServiceCollectionExtensions).Assembly.GetTypes();

        var interfaces = types
            .Where(xx => xx.Name.EndsWith("Service") && xx.IsInterface);

        foreach (var abstraction in interfaces)
        {
            var implementation = types.FirstOrDefault(xx => xx.IsAssignableFrom(abstraction) && !xx.IsAbstract && !xx.IsInterface);

            if (implementation is null) continue;

            services.AddTransient(abstraction, implementation);
        }

        return services;
    }
}
