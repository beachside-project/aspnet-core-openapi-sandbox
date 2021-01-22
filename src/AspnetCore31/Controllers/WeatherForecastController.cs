using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspnetCore31.Controllers
{
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
        }

        [HttpGet]
        [Produces("application/json")]
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
        /// 指定したカウントだけサンプルの天気予報データを返します。
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///       "id": 1,
        ///       "name": "Item1",
        ///       "isComplete": true
        ///     }
        /// </remarks>
        /// <param name="count">天気予報データの件数 (1件以上、100件未満)</param>
        /// <returns>天気予報データ</returns>
        [HttpGet("Get2")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), 200)]
        [ProducesResponseType(400)]
        public ActionResult<IEnumerable<WeatherForecast>> Get2(int count)
        {
            if (count < 1 || count > 100)
            {
                return BadRequest();
            }

            var rng = new Random();
            return Enumerable.Range(1, count).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
                .ToArray();
        }

        /// <summary>
        /// WeatherForecast を登録する
        /// </summary>
        /// <param name="weatherForecast"></param>
        /// <returns></returns>
        /// <response code="200">WeatherForecast created</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(WeatherForecast), 200)]
        [ProducesResponseType(typeof(Dictionary<string, string>), 400)]
        [HttpPost]
        public async Task<ActionResult<WeatherForecast>> Post(WeatherForecast weatherForecast)
        {
            if (weatherForecast == null)
            {
                return BadRequest();
            }
            await Task.Delay(1000);

            return new OkObjectResult(weatherForecast);
        }


        [HttpGet]
        [Produces("application/json")]
        [Route("{location}/weather")]
        public IEnumerable<WeatherForecast> Get3(string location, int count)
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
    }
}
