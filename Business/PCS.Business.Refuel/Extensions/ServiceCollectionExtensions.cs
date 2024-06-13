using PCS.Common.Business.Extensions;

namespace PCS.Business.Refuel.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRefuel(this IServiceCollection services)
    {
        return services.AddServices<Services.RefuelService>();
    }
}
