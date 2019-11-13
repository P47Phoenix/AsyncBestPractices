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
    public class SyncToAsyncWeatherController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly Lazy<ThreadPoolLogger> _lazyThreadPoolLogger;

        public SyncToAsyncWeatherController(IHttpClientFactory clientFactory, Lazy<ThreadPoolLogger> lazyThreadPoolLogger)
        {
            _clientFactory = clientFactory;
            _lazyThreadPoolLogger = lazyThreadPoolLogger;
        }
        
        public IActionResult Index()
        {
            try
            {
                var httpClient = _clientFactory.CreateClient("someapi");

                using (HttpRequestMessage httpRequestMessage =
                    new HttpRequestMessage(HttpMethod.Get, "weatherforecast"))
                {
                    var result = httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false).GetAwaiter().GetResult();

                    if (result.IsSuccessStatusCode == false)
                    {
                        return StatusCode(500);
                    }

                    JsonSerializer js = new JsonSerializer();

                    using (var stream = result.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult())
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