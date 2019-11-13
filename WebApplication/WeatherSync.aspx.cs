using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;
using System.Net;
using TestApiCall;

namespace WebApplication
{
    public partial class WeatherSync : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
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
                        m_datagrid_weather.DataSource = data;
                        m_datagrid_weather.DataBind();
                    }
                }
            }
            catch (WebException webException)
            {
                Global.ThreadPoolLogger.ErrorOccured();
                Log.Error(webException, "Api call failed");
            }
            catch (Exception exception)
            {
                Global.ThreadPoolLogger.ErrorOccured();
                Log.Error(exception, "A unknown error was thrown");
            }
        }
    }
}