using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Serilog;
using TestApiCall;
using Util.Thread;

namespace WebApplication
{
    public partial class WeatherSyncToCustomContext : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                AsyncHelpers.RunSync( async () =>
                {
                    using (var cancellationTokenSource = new CancellationTokenSource(500))
                    {
                        var httpClient = new HttpClientFactory().Create(new Uri(Properties.Settings.Default.RootUri));

                        using (HttpRequestMessage httpRequestMessage =
                            new HttpRequestMessage(HttpMethod.Get, "weatherforecast"))
                        {

                            var result = await httpClient.SendAsync(httpRequestMessage, cancellationTokenSource.Token).ConfigureAwait(false);

                            if (result.IsSuccessStatusCode == false)
                            {
                                return;
                            }

                            JsonSerializer js = new JsonSerializer();

                            using (var stream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            using (StreamReader reader = new StreamReader(stream))
                            using (JsonTextReader jsonTextReader = new JsonTextReader(reader))
                            {
                                var data = js.Deserialize<WeatherForecast[]>(jsonTextReader);
                                m_datagrid_weather.DataSource = data;
                                m_datagrid_weather.DataBind();
                            }
                        }
                    }

                });
            }
            catch (Exception error)
            {
                Global.ThreadPoolLogger.ErrorOccured();
                Log.Error(error, "error getting weather");

            }
        }
    }
}