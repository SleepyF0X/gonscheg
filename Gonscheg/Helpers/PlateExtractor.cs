using System.Text.RegularExpressions;

namespace Gonscheg.Helpers;

/// <summary>
/// A utility class for extracting and validating vehicle license plates.
/// The class is partial to support source-generated regular expressions.
/// </summary>
public static partial class PlateExtractor
{
    private static readonly string[] BannedSubstrings = ["СБУ", "OO", "BP", "KM"];

    /// <summary>
    /// Extract a license plate from a message.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <returns>A license plate (can be invalid)</returns>
    public static string? ExtractPlateFromNashessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return null;
        }

        var match = MessagePlateRegex().Match(message);

        if (!match.Success)
        {
            return null;
        }

        var plate = match.Groups[1].Value.Trim();
        return plate;
    }

    public static string? ExtractPlateFromFaFaMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return null;
        }

        var match = MessagePlateRegex().Match(message);

        if (!match.Success)
        {
            return null;
        }

        var plate = match.Groups[1].Value.Trim();
        return plate;
    }

    /// <summary>
    /// Checks if a string is a valid vehicle license plate.
    /// </summary>
    /// <param name="plate">The string to check.</param>
    /// <returns>True if the plate is valid, otherwise false.</returns>
    public static bool IsPlateValid(string plate)
    {
        // Special case for 4-digit plates
        if (plate.All(char.IsDigit) && plate.Length == 4)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(plate))
            return false;

        // Prepare a "compact" version of the plate for checks (uppercase, no whitespace)
        var compactPlate = new string(plate.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToUpperInvariant();

        // Using pattern matching for a more concise check
        if (compactPlate.Length is < 3 or > 8)
            return false;

        // Check for standard plates (e.g., AA1234AA)
        if (StandardPlateRegex().IsMatch(compactPlate))
            return true;

        // Check for custom plates
        if (!CustomPlateRegex().IsMatch(compactPlate))
            return false;

        if (!char.IsLetter(compactPlate[0]))
            return false;

        if (compactPlate.Count(char.IsDigit) > 6)
            return false;

        if (BannedSubstrings.Any(compactPlate.Contains))
            return false;

        return true;
    }

    // Using source-generated regular expressions for better performance.
    [GeneratedRegex(@"наш\??\s+([А-ЯA-Z0-9 ]{0,10})\??", RegexOptions.IgnoreCase)]
    private static partial Regex MessagePlateRegex();

    [GeneratedRegex(@"^[А-ЯA-Z]{2}\d{4}[А-ЯA-Z]{2}$", RegexOptions.IgnoreCase)]
    private static partial Regex StandardPlateRegex();

    [GeneratedRegex(@"^[А-ЯA-Z0-9]{3,8}$", RegexOptions.IgnoreCase)]
    private static partial Regex CustomPlateRegex();
}