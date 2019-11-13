using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Serilog;
using TestApiCall;
using WebApplication;

namespace MvcWebApplication.Controllers
{
    public class SyncWeatherController : Controller
    {
        
        public ActionResult Index()
        {
            var request = System.Net.WebRequest.CreateHttp(new Uri($"{Properties.Settings.Default.RootUri}weatherforecast"));

            request.Method = "GET";
            try
            {
                using (var response = request.GetResponse())
                {
                    JsonSerializer js = new JsonSerializer();

                    using (var stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    using (JsonTextReader jsonTextReader = new JsonTextReader(reader))
                    {
                        var data = js.Deserialize<WeatherForecast[]>(jsonTextReader);

                        return View(data);
                    }
                }
            }
            catch (WebException webException)
            {
                WebApiApplication.ThreadPoolLogger.ErrorOccured();
                Log.Error(webException, "Api call failed");
                return new HttpStatusCodeResult(500);
            }
            catch (Exception exception)
            {
                WebApiApplication.ThreadPoolLogger.ErrorOccured();
                Log.Error(exception, "A unknown error was thrown");
                return new HttpStatusCodeResult(500);
            }
        }
    }
}