using System.Globalization;
using System.Text;

namespace RENHelpers.StringHelper.ExtensionsFolder;

public static class StringExtensions
{
    public static bool RENIsNullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool RENIsNullOrWhitespace(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static bool RENIsValid(this string value)
    {
        return !string.IsNullOrEmpty(value) || !string.IsNullOrWhiteSpace(value);
    }

    public static string RENFormatWithArgs(this string value, params object[] args)
    {
        return string.Format(value, args);
    }

    public static bool RENCompareIgnoreCase(this string value, string valueToCompare)
    {
        return string.Equals(value, valueToCompare, StringComparison.OrdinalIgnoreCase);
    }

    public static bool RENCompare(this string value, string valueToCompare)
    {
        return string.Equals(value, valueToCompare, StringComparison.Ordinal);
    }

    public static string RENRemoveDiacritics(this string value)
    {
        var normalizedString = value.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in from c in normalizedString let unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c) where unicodeCategory != UnicodeCategory.NonSpacingMark select c)
        {
            stringBuilder.Append(c);
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string RENRemoveSpecialCharacters(this string value)
    {
        var sb = new StringBuilder();
        foreach (var c in value.Where(c => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') ||
                                           c is >= 'a' and <= 'z' || c == ' '))
        {
            sb.Append(c);
        }
        return sb.ToString();
    }

    public static string[] RENSplitString(this string input, char separator)
    {
        return input.Split(separator);
    }

    public static string RENTrimWhitespace(this string input)
    {
        return input.Trim();
    }

    public static string RENTrimCharacters(this string input, char[] characters)
    {
        return input.Trim(characters);
    }
}