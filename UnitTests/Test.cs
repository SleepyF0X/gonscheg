using System.Text;
using System.Text.RegularExpressions;
using Gonscheg.Helpers;
using WeatherAPI_CSharp;

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
        var text = "/reg\n1. Хуй с горы\n2. КА0912КТ\n3. 10.08.2001";
        Assert.Pass();
    }
}
