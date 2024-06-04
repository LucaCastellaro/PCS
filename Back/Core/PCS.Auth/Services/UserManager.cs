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
}

public class UserManager : IUserManager
{
    private readonly IAuth0Client _auth0Client;
    private readonly string _domain;
    private readonly string _clientId;
    private ClaimsPrincipal? _principal;

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

        _principal = loginResult.User;

        return _principal;
    }

    public async Task Logout()
    {
        if (!await IsLogged()) return;
        await _auth0Client.LogoutAsync();
        _principal = null;
        SecureStorage.Default.RemoveAll();
    }

    public async Task<ClaimsPrincipal?> GetAuthenticatedUser()
    {
        if (_principal is not null) return _principal;

        var idToken = await SecureStorage.Default.GetAsync(Constants.SecureStorageKeys.IDENTITY_TOKEN);

        if (idToken is null) return _principal;

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

        if (validationResult.IsError) return _principal;

        _principal = validationResult.User;
        return _principal;
    }

    public async Task<bool> IsLogged()
    {
        if (_principal is not null) return true;

        var idToken = await SecureStorage.Default.GetAsync(Constants.SecureStorageKeys.IDENTITY_TOKEN);

        return idToken != null;
    }

    public async Task<string?> GetUserName()
    {
        if (_principal is null)
        {
            _principal = await GetAuthenticatedUser();
            if (_principal is null) return null;
        }
        return _principal.Claims.FirstOrDefault(xx => xx.Type == "given_name")?.Value
                    ?? _principal.Identity!.Name;
    }
}