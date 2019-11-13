using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Serilog;
using TestApiCall;

namespace WebApplication
{
    public partial class WeatherSyncToTask : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Task.Run(async () =>
                {
                var httpClient = new HttpClientFactory().Create(new Uri(Properties.Settings.Default.RootUri));

                using (HttpRequestMessage httpRequestMessage =
                    new HttpRequestMessage(HttpMethod.Get, "weatherforecast"))
                {

                    var result = await httpClient.SendAsync(httpRequestMessage);

                    if (result.IsSuccessStatusCode == false)
                    {
                        return;
                    }

                    JsonSerializer js = new JsonSerializer();

                    using (var stream = await result.Content.ReadAsStreamAsync())
                    using (StreamReader reader = new StreamReader(stream))
                    using (JsonTextReader jsonTextReader = new JsonTextReader(reader))
                    {
                        var data = js.Deserialize<WeatherForecast[]>(jsonTextReader);
                        m_datagrid_weather.DataSource = data;
                        m_datagrid_weather.DataBind();
                    }
                }
                

                }).GetAwaiter().GetResult();

            }
            catch (Exception error)
            {
                Global.ThreadPoolLogger.ErrorOccured();
                Log.Error(error, "error getting weather");
            }
        }
    }
}