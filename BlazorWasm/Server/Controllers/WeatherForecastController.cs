using BlazorWasm.Client.Models;
using BlazorWasm.Client.Services;

using Microsoft.AspNetCore.Mvc;

namespace BlazorWasm.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForecastService _service;

    public WeatherForecastController(IWeatherForecastService service)
    {
        _service = service;
    }

    [HttpGet]
    public Task<IEnumerable<WeatherForecast>?> GetAsync() =>
        _service.GetForecastAsync(DateOnly.FromDateTime(DateTime.Now));
}
