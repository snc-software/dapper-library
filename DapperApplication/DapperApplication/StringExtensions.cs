namespace DapperApplication;

public static class StringExtensions
{
    public static string Pluralise(this string value)
    {
        return value.EndsWith('s') ? value : $"{value}s";
    }
}