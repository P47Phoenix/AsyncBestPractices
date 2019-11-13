using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using TestApiCall;
using WebApplication;

namespace CoreWebApplication.Controllers
{
    public class AsyncWeatherController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly Lazy<ThreadPoolLogger> _lazyThreadPoolLogger;

        public AsyncWeatherController(IHttpClientFactory clientFactory, Lazy<ThreadPoolLogger> lazyThreadPoolLogger)
        {
            _clientFactory = clientFactory;
            _lazyThreadPoolLogger = lazyThreadPoolLogger;
        }
        
        public async Task<IActionResult> Index()
        {
            try
            {
                var httpClient = _clientFactory.CreateClient("someapi");

                using (HttpRequestMessage httpRequestMessage =
                    new HttpRequestMessage(HttpMethod.Get, "weatherforecast"))
                {
                    var result = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

                    if (result.IsSuccessStatusCode == false)
                    {
                        return StatusCode(500);
                    }

                    JsonSerializer js = new JsonSerializer();

                    using (var stream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (StreamReader reader = new StreamReader(stream))
                    using (JsonTextReader jsonTextReader = new JsonTextReader(reader))
                    {
                        var data = js.Deserialize<WeatherForecast[]>(jsonTextReader);
                        return View(data);
                    }
                }
                
            }
            catch (Exception error)
            {
                _lazyThreadPoolLogger.Value.ErrorOccured();
                Log.Error(error, "error getting weather");
                return StatusCode(500);
            }
        }
    }
}