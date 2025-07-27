using Gonscheg.Shared.Exceptions;

namespace Gonscheg.Shared;

public static class EnvironmentVariables
{
    public static string TelegramBotToken { get; private set; }
    public static string DBHost { get; private set; }
    public static string DBUser { get; private set; }
    public static string DBPass { get; private set; }
    public static string WeatherAPIKey { get; private set; }

    public static void InitializeEnvironmentVariables()
    {
        TelegramBotToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") ??
                           throw new EnvVariableNullException("TELEGRAM_BOT_TOKEN");
        DBHost = Environment.GetEnvironmentVariable("DB_HOST") ??
                 throw new EnvVariableNullException("DB_HOST");
        DBUser = Environment.GetEnvironmentVariable("DB_USER") ??
                 throw new EnvVariableNullException("DB_USER");
        DBPass = Environment.GetEnvironmentVariable("DB_PASS") ??
                 throw new EnvVariableNullException("DB_PASS");
        WeatherAPIKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY") ??
                        throw new EnvVariableNullException("WEATHER_API_KEY");
    }
}