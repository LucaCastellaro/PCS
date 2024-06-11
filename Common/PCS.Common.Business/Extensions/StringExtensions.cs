namespace PCS.Common.Business.Extensions;
public static class StringExtensions
{
    public static string Sanitize(this string? source) => source?.Trim().ToUpper() ?? string.Empty;
}
