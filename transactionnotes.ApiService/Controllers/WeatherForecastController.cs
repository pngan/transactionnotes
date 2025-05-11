using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace transactionnotes.ApiService.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
[Authorize(Roles = "transactionnotes-write")] // This applies authorization to all endpoints in the controller
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet(Name = "GetWeatherForecast")]
    public IActionResult Get()
    {
        //if (!User.Identity?.IsAuthenticated ?? false)
        //{
            //return Unauthorized();
     //   }

        var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();

        return Ok(forecasts);
    }
}

public class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
