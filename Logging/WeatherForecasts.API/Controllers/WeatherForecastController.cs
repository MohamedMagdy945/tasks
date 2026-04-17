using Microsoft.AspNetCore.Mvc;

namespace WeatherForecasts.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        //private readonly ILogger<WeatherForecastController> _logger;
        //public WeatherForecastController(ILogger<WeatherForecastController> logger)
        //{
        //    _logger = logger;
        //}

        //[HttpGet(Name = "GetWeatherForecast")]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    _logger.LogInformation("Starting info Get weather forecast");
        //    _logger.LogDebug("Getting debuggg weather forecast for the next 5 days");

        //    for (var i = 0; i < 5000000; i++)
        //    {
        //        _logger.LogDebug("Weather forecasts for {i}", i);
        //    }

        //    try
        //    {
        //        throw new Exception("eerror");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while getting weather forecast");
        //    }

        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();

        //}
        //[HttpPost]
        //public void Post(WeatherForecast weatherForecast)
        //{
        //    _logger.LogInformation("Received weather forecast: {@WeatherForecast}", weatherForecast);
        //}
    }
}
