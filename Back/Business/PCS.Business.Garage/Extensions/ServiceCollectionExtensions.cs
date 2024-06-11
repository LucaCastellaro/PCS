namespace PCS.Business.Garage.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGarage(this IServiceCollection services)
    {
        services.AddServices();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        var types = typeof(ServiceCollectionExtensions).Assembly.GetTypes();

        var interfaces = types.Where(xx => xx.Name.StartsWith("I") && xx.Name.EndsWith("Service") && xx.IsInterface);

        foreach (var abstraction in interfaces)
        {
            var implementation = types.FirstOrDefault(xx => xx.Name == abstraction.Name[1..] && xx.IsClass && !xx.IsAbstract);

            if (implementation is null) continue;

            services.AddTransient(abstraction, implementation);
        }

        return services;
    }
}
