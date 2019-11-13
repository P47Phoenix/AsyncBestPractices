
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using Serilog;
using TestApiCall;
using HttpClientFactory = WebApplication.HttpClientFactory;

namespace MvcWebApplication.Controllers
{
    public class AsyncWeatherController : Controller
    {
        
        public async Task<ActionResult> Index()
        {
            try
            {
                var httpClient = new HttpClientFactory().Create(new Uri(Properties.Settings.Default.RootUri));

                using (HttpRequestMessage httpRequestMessage =
                    new HttpRequestMessage(HttpMethod.Get, "weatherforecast"))
                {
                    var result = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

                    if (result.IsSuccessStatusCode == false)
                    {
                        return new HttpStatusCodeResult(500);
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
                WebApiApplication.ThreadPoolLogger.ErrorOccured();
                Log.Error(error, "error getting weather");
                return new HttpStatusCodeResult(500);
            }
        }
    }
}
