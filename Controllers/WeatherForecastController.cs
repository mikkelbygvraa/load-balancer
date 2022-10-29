using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

namespace LoadBalancer.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;

        var hostName = Dns.GetHostName();
        var ips = Dns.GetHostAddresses(hostName);
        var ipa = ips.First().MapToIPv4().ToString();

        _logger.LogInformation(1, $"WeatherForecast responding from {ipa}");
    }

    // ------------------------------------------------------------------------------

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }


    [HttpGet("version")]
    public async Task<Dictionary<string, string>> GetVersion()
    {
        var properties = new Dictionary<string, string>();
        var assembly = typeof(Program).Assembly;

        properties.Add("service", "Weather Forecast");
        
        var ver = FileVersionInfo.GetVersionInfo(typeof(Program).Assembly.Location).ProductVersion;
       
        properties.Add("version", ver);

        var hostName = Dns.GetHostName();
        var ips = await Dns.GetHostAddressesAsync(hostName);
        var ipa = ips.First().MapToIPv4().ToString();

        properties.Add("ip-address", ipa);

        return properties;
    }


    [HttpGet("station")]
    public IEnumerable<WeatherForecast> GetStationData(int stationId)
    {
        var theData = new List<WeatherForecast>();
        var clientIP = HttpContext.Connection.RemoteIpAddress?.MapToIPv4();

        Console.WriteLine("");

        if (2 < stationId)
        {
            _logger.LogWarning($"Wrong station Id request from {clientIP}");
        }
        else
        {
            _logger.LogDebug($"Returning weather data for station no. { stationId }");

            return Enumerable.Range(1, 2).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        return theData;
    }
}
