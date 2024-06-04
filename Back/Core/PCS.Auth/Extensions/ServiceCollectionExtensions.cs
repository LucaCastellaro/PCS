using Auth0.OidcClient;
using PCS.Auth.Models;
using PCS.Auth.Services;

namespace PCS.Auth.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuth0<T>(this IServiceCollection services)
        where T : class
    {
        services.AddSingleton<T>();

        var client = new Auth0Client(new()
        {
            Domain = Constants.AuthSettings.Domain,
            ClientId = Constants.AuthSettings.ClientId,
            RedirectUri = Constants.AuthSettings.RedirectUri,
            PostLogoutRedirectUri = Constants.AuthSettings.PostLogoutRedirectUri,
            Scope = string.Join(' ', Constants.AuthSettings.Scopes),
        });

        services.AddSingleton<IAuth0Client>(client);

        services.AddSingleton<IUserManager>(new UserManager(Constants.AuthSettings.Domain, Constants.AuthSettings.ClientId, client));

        return services;
    }
}