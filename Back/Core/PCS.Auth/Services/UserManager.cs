using Auth0.OidcClient;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using PCS.Auth.Models;
using System.Security.Claims;

namespace PCS.Auth.Services;

public interface IUserManager
{
    Task<ClaimsPrincipal?> GetAuthenticatedUser();

    Task<ClaimsPrincipal?> Login();

    Task Logout();

    Task<string?> GetUserName();

    Task<bool> IsLogged();

    ClaimsPrincipal? Principal { get; }
}

public class UserManager : IUserManager
{
    private readonly IAuth0Client _auth0Client;
    private readonly string _domain;
    private readonly string _clientId;
    public ClaimsPrincipal? Principal { get; private set; }

    public UserManager(string domain, string clientId, IAuth0Client auth0Client)
    {
        _domain = domain;
        _clientId = clientId;
        _auth0Client = auth0Client;
    }

    public async Task<ClaimsPrincipal?> Login()
    {
        var user = await GetAuthenticatedUser();
        if (user is not null) return user;

        var loginResult = await _auth0Client.LoginAsync();

        if (loginResult.IsError)
        {
            throw new UnauthorizedAccessException(loginResult.Error);
        }
        await SecureStorage.Default.SetAsync(Constants.SecureStorageKeys.IDENTITY_TOKEN, loginResult.IdentityToken);
        await SecureStorage.Default.SetAsync(Constants.SecureStorageKeys.ACCESS_TOKEN, loginResult.AccessToken);

        Principal = loginResult.User;

        return Principal;
    }

    public async Task Logout()
    {
        if (!await IsLogged()) return;
        await _auth0Client.LogoutAsync();
        Principal = null;
        SecureStorage.Default.RemoveAll();
    }

    public async Task<ClaimsPrincipal?> GetAuthenticatedUser()
    {
        if (Principal is not null) return Principal;

        var idToken = await SecureStorage.Default.GetAsync(Constants.SecureStorageKeys.IDENTITY_TOKEN);

        if (idToken is null) return Principal;

        var doc = await new HttpClient().GetDiscoveryDocumentAsync($"https://{_domain}");
        var validator = new JwtHandlerIdentityTokenValidator();
        var options = new OidcClientOptions
        {
            ClientId = _clientId,
            ProviderInformation = new ProviderInformation
            {
                IssuerName = doc.Issuer,
                KeySet = doc.KeySet
            }
        };

        var validationResult = await validator.ValidateAsync(idToken, options);

        if (validationResult.IsError) return Principal;

        Principal = validationResult.User;
        return Principal;
    }

    public async Task<bool> IsLogged()
    {
        if (Principal is not null) return true;

        return await GetAuthenticatedUser() is not null;
    }

    public async Task<string?> GetUserName()
    {
        if (Principal is null)
        {
            Principal = await GetAuthenticatedUser();
            if (Principal is null) return null;
        }
        return Principal.Claims.FirstOrDefault(xx => xx.Type == "given_name")?.Value
                    ?? Principal.Identity!.Name;
    }
}