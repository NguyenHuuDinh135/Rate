using Refit;

namespace WebUI.Shared.Services.Api;

public interface IWeatherApi
{
    [Get("/weatherforecast")]
    Task<IEnumerable<WeatherForecast>> GetWeatherAsync();
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
