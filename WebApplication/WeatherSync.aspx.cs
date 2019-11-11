using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Serilog;
using TestApiCall;

namespace WebApplication
{
    public partial class WeatherSync : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var request = WebRequest.CreateHttp(new Uri("https://localhost:44363/weatherforecast"));

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
                Log.Error(webException, "Api call failed");
            }
            catch (Exception exception)
            {
                Log.Error(exception, "A unknown error was thrown");
            }
        }
    }
}