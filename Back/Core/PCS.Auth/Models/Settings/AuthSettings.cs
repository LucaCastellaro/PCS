namespace PCS.Auth.Models.Settings;
public sealed record AuthSettings
{
    public required string Domain { get; init; }
    public required string ClientId { get; init; }
    public required string RedirectUri { get; init; }
    public required string PostLogoutRedirectUri { get; init; }
    public required string[] Scopes { get; init; }
}