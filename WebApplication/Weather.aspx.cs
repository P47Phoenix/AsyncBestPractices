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
    public partial class Weather : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpClient httpClient = new HttpClient();

            HttpRequestMessage httpRequestMessage =
                new HttpRequestMessage(HttpMethod.Get, "https://localhost:44363/weatherforecast");

            var result = httpClient.SendAsync(httpRequestMessage)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (result.IsSuccessStatusCode == false)
            {
                return;
            }

            JsonSerializer js = new JsonSerializer();

            using (var stream = result.Content.ReadAsStreamAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult())
            using (StreamReader reader = new StreamReader(stream))
            using (JsonTextReader jsonTextReader = new JsonTextReader(reader))
            {
                var data = js.Deserialize<WeatherForecast[]>(jsonTextReader);
                m_datagrid_weather.DataSource = data;
                m_datagrid_weather.DataBind();
            }
        }
    }
}