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
using TestApiCall;

namespace WebApplication
{
    public partial class WeatherAsync : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                HttpClient httpClient = new HttpClient();

                HttpRequestMessage httpRequestMessage =
                    new HttpRequestMessage(HttpMethod.Get, "https://localhost:44363/weatherforecast");

                var result = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

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
            }));
            this.ExecuteRegisteredAsyncTasks();
        }
    }
}