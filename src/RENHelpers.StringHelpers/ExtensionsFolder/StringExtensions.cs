namespace RENHelpers.StringHelper.ExtensionsFolder;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsNullOrWhitespace(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static bool IsValid(this string value)
    {
        return !string.IsNullOrEmpty(value) || !string.IsNullOrWhiteSpace(value);
    }

    public static string FormatWithArgs(this string value, params object[] args)
    {
        return string.Format(value, args);
    }

    public static bool CompareIgnoreCase(this string value, string valueToCompare)
    {
        return string.Equals(value, valueToCompare, StringComparison.OrdinalIgnoreCase);
    }

    public static bool Compare(this string value, string valueToCompare)
    {
        return string.Equals(value, valueToCompare, StringComparison.Ordinal);
    }

    public static string[] Split(this string value, string separator)
    {
        return value.Split(new[] { separator }, StringSplitOptions.None);
    }
}