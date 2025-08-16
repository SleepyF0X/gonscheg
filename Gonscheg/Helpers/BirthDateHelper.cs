using System.Globalization;

namespace Gonscheg.Helpers;

public class BirthDateHelper
{
    public static bool TryParseDateWithDefaultYear(string dateString, int defaultYear, out DateTime? birthDate)
    {
        birthDate = null;
        if (DateTime.TryParseExact(dateString, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fullDate))
        {
            birthDate = fullDate;
            return true;
        }

        if (DateTime.TryParseExact(dateString, "dd.MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var partialDate))
        {
            birthDate = new DateTime(defaultYear, partialDate.Month, partialDate.Day);
            return true;
        }

        return false;
    }
}
