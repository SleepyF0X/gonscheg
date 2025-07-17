using System.Text.RegularExpressions;

namespace Gonscheg.Helpers;

public static class PlateExtractor
{
    public static string? TryExtractPlateFromMessage(string message, out bool isPlateValid)
    {
        isPlateValid = false;

        if (string.IsNullOrWhiteSpace(message))
            return null;

        var regex = new Regex(@"наш\??\s+([А-ЯA-Z0-9 ]{0,10})\??", RegexOptions.IgnoreCase);
        var match = regex.Match(message);

        if (!match.Success)
            return null;

        var candidate = match.Groups[1].Value.Trim();

        if (IsPlateValid(candidate))
        {
            isPlateValid = true;
        }

        return candidate;
    }

    public static bool IsPlateValid(string plate)
    {
        if (plate.All(char.IsDigit) && plate.Length == 4)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(plate))
            return false;

        plate = plate.Trim().ToUpperInvariant();

        var compactPlate = new string(plate.Where(c => !char.IsWhiteSpace(c)).ToArray());
        if (compactPlate.Length < 3 || compactPlate.Length > 8)
            return false;

        var standardPattern = new Regex(@"^[А-ЯA-Z]{2}\d{4}[А-ЯA-Z]{2}$", RegexOptions.IgnoreCase);
        if (standardPattern.IsMatch(compactPlate))
            return true;

        var customPattern = new Regex(@"^[А-ЯA-Z0-9]{3,8}$", RegexOptions.IgnoreCase);
        if (!customPattern.IsMatch(compactPlate))
            return false;

        if (!char.IsLetter(compactPlate[0]))
            return false;

        var digitCount = compactPlate.Count(char.IsDigit);
        if (digitCount > 6)
            return false;

        string[] bannedSubstrings = { "СБУ", "OO", "BP", "KM" };
        if (bannedSubstrings.Any(b => compactPlate.Contains(b)))
            return false;

        return true;
    }
}