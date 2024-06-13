namespace PCS.Common.Business.Extensions;
public static class DecimalExtensions
{
    public static decimal Round(this decimal value, uint trailingDigits) => Math.Round(value, (int)trailingDigits, MidpointRounding.ToZero);
}
