using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Serilog;
using TestApiCall;

namespace WebApplication
{
    public partial class WeatherAsync : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.RegisterAsyncTask(new PageAsyncTask(async token =>
            {
                try
                {
                    var httpClient = new HttpClientFactory().Create(new Uri(Properties.Settings.Default.RootUri));

                    using (HttpRequestMessage httpRequestMessage =
                        new HttpRequestMessage(HttpMethod.Get, "weatherforecast"))
                    {
                        var result = await httpClient.SendAsync(httpRequestMessage, token).ConfigureAwait(false);

                        if (result.IsSuccessStatusCode == false)
                        {
                            return;
                        }

                        JsonSerializer js = new JsonSerializer();

                        using (var stream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        using (StreamReader reader = new StreamReader(stream))
                        using (JsonTextReader jsonTextReader = new JsonTextReader(reader))
                        {
                            m_datagrid_weather.DataSource = js.Deserialize<WeatherForecast[]>(jsonTextReader);
                            m_datagrid_weather.DataBind();
                        }
                    }
                
                }
                catch (Exception error)
                {
                    Global.ThreadPoolLogger.ErrorOccured();
                    Log.Error(error, "error getting weather");
                }
            }));

            this.ExecuteRegisteredAsyncTasks();
        }
    }
}