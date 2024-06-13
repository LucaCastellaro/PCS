using PCS.Common.Business.Extensions;

namespace PCS.Business.Garage.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGarage(this IServiceCollection services)
    {
        return services.AddServices<Services.VehicleService>();
    }
}
