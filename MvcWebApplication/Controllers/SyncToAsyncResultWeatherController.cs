using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Serilog;
using TestApiCall;
using WebApplication;

namespace MvcWebApplication.Controllers
{
    public class SyncToAsyncResultWeatherController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                var httpClient = new WebApplication.HttpClientFactory().Create(new Uri(MvcWebApplication.Properties.Settings.Default.RootUri));

                using (HttpRequestMessage httpRequestMessage =
                    new HttpRequestMessage(HttpMethod.Get, "weatherforecast"))
                {
                    var result = httpClient.SendAsync(httpRequestMessage).Result;

                    if (result.IsSuccessStatusCode == false)
                    {
                        return new HttpStatusCodeResult(500);
                    }

                    JsonSerializer js = new JsonSerializer();

                    using (var stream = result.Content.ReadAsStreamAsync().Result)
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