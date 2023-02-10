using Microsoft.AspNetCore.Mvc;
using RedisCache.Attributes;
using RedisCache.Services;

namespace RedisCache.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    private readonly IResponseCacheService _responseCacheService;
    public WeatherForecastController(ILogger<WeatherForecastController> logger , IResponseCacheService responseCacheService)
    {
        _logger = logger;
        _responseCacheService = responseCacheService;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    [Cache(100)]
    public OkObjectResult Get()
    {
        Console.WriteLine("da vao");
        return new OkObjectResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray());
    }

    [HttpPost]
    public async Task<IActionResult> Create(string name)
    {
        Summaries.Append<string>(name);

        await  _responseCacheService.RemoveCacheResponseAsync(HttpContext.Request.Path);

        return Ok();
    }
}
