using System.Security.Claims;

namespace PCS.Auth.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Claim? GetClaim(this ClaimsPrincipal principal, string key) => principal.Claims.FirstOrDefault(xx => xx.Type == key);
    public static string? GetClaimValue(this ClaimsPrincipal principal, string key) => principal.GetClaim(key)?.Value;
    public static string? GetFirstName(this ClaimsPrincipal principal) => principal.GetClaim("given_name")?.Value;
    public static string? GetLastName(this ClaimsPrincipal principal) => principal.GetClaim("family_name")?.Value;
    public static string? GetFullName(this ClaimsPrincipal principal) => principal.GetClaim("name")?.Value;
    public static string? GetUserId(this ClaimsPrincipal principal) => principal.GetClaim("sid")?.Value;
}
