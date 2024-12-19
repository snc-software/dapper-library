namespace SncSoftware.Extensions.Dapper.Extensions;

internal static class StringExtensions
{
    public static string Pluralise(this string value)
    {
        return value.EndsWith('s') ? value : $"{value}s";
    }
}