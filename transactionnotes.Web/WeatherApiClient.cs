namespace transactionnotes.Web;

public class WeatherApiClient(HttpClient httpClient)
{
    public async Task<WeatherForecast[]> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        // Make the request and check for errors before streaming
        var response = await httpClient.GetAsync("/api/v1/weatherforecast", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Backend returned error: {(int)response.StatusCode} {response.ReasonPhrase}\n{content}", null, response.StatusCode);
        }

        List<WeatherForecast>? forecasts = null;
        await foreach (var forecast in response.Content.ReadFromJsonAsAsyncEnumerable<WeatherForecast>(cancellationToken: cancellationToken))
        {
            if (forecasts?.Count >= maxItems)
            {
                break;
            }
            if (forecast is not null)
            {
                forecasts ??= [];
                forecasts.Add(forecast);
            }
        }

        return forecasts?.ToArray() ?? [];
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
