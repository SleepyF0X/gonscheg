using System.Text;
using Gonscheg.Shared;
using WeatherAPI_CSharp;

namespace Gonscheg.Helpers;

public class WeatherClient
{
    private readonly APIClient _weatherClient = new(EnvironmentVariables.WeatherAPIKey);
    public async Task<string> GetFutureDayWeatherAsync(DateTime now, string city)
    {
        var forecast = await _weatherClient.GetWeatherForecastHourlyAsync(city);
        var sb = new StringBuilder();
        sb.AppendLine("Погода на сегодня (но это не точно): ");
        foreach (var hourly in forecast)
        {
            if (hourly.Date.Hour < now.Hour || hourly.Date.Hour > 23)
            {
                continue;
            }
            var icon = hourly.ChanceOfRain > 50 ? "🌧" :
                hourly.ChanceOfRain > 20 ? "🌦" :
                hourly.TemperatureCelsius < 19 ? "☁️" :
                hourly.TemperatureCelsius < 21 ? "🌥" :
                hourly.TemperatureCelsius < 22.5 ? "🌤" : "☀️";

            sb.AppendLine($"{hourly.Date.Hour} — {icon} {hourly.TemperatureCelsius:F1}°C, 💧— {hourly.ChanceOfRain}%");
        }

        return sb.ToString();
    }
}