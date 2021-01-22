using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspnetCore50.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
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
        }

        /// <summary>
        /// 天気予報のサンプルデータを5件返します。
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetWeather")]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }


        /// <summary>
        /// 指定した件数の天気予報のサンプルデータを返します。
        /// </summary>
        /// <remarks>
        /// Sample input:
        ///  
        /// </remarks>
        /// <param name="count">出力したい件数</param>
        /// <returns>サンプルの天気予報データ</returns>
        [HttpGet("2")]
        public IActionResult Get2(int count)
        {
            if (count < 1)
            {
                return BadRequest();
            }
            var rng = new Random();
            var weatherForecasts = Enumerable.Range(1, count).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
                .ToArray();

            return new OkObjectResult(weatherForecasts);
        }

        [HttpPost]
        public ActionResult<WeatherForecast> Post(WeatherForecast weatherForecast)
        {
            return new CreatedAtRouteResult("GetWeather", null, weatherForecast);
        }
    }
}