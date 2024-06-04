using PCS.Auth.Models.Settings;

namespace PCS.Auth.Models;

internal static class Constants
{
    internal static readonly AuthSettings AuthSettings = new()
    {
        ClientId = "uQAiuCExgRs52FHS4YcZFmUMhibIE721",
        Domain = "dev-pkw2obkoio32kbu4.eu.auth0.com",
        PostLogoutRedirectUri = "myapp://callback/",
        RedirectUri = "myapp://callback/",
        Scopes = ["openid", "profile", "email"]
    };

    internal static class SecureStorageKeys
    {
        internal const string ACCESS_TOKEN = nameof(ACCESS_TOKEN);
        internal const string IDENTITY_TOKEN = nameof(IDENTITY_TOKEN);
    }
}