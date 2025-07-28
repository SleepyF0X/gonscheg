using System.Text.RegularExpressions;

namespace UnitTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Test()
    {
        var messageText = "/reg Д + КА0360КТ + + 10.08.2001";
        var match = Regex.Match(
            messageText,
            @"\/reg\s+([A-ZА-ЯІЇЄ0-9]+)\s+\+\s+([A-ZА-ЯІЇЄ0-9]+)\s+\+((\s+([A-Z0-9]{0,17})\s+)|\s+)\+\s+(\d{2}\.\d{2}\.\d{4})",
            RegexOptions.IgnoreCase);
        var name = match.Groups[1].Value;
        var plate = match.Groups[2].Value;
        var vinCode = !string.IsNullOrEmpty(match.Groups[3].Value.Trim()) ? match.Groups[2].Value.Trim() : null;
        var isBirthDateValid = DateTime.TryParse(match.Groups[6].Value, out var birthdate);
        Assert.Pass();
    }
}