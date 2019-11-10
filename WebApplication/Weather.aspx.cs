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
            if (this.IsAsync)
            {
                // this does not execute the async task.
                // it only puts it in a queue that we get call before page render
                this.RegisterAsyncTask(new PageAsyncTask(AsyncCall));
                // So you if you want to control when it runs call
                // forces execution of all registered async tasks
                this.ExecuteRegisteredAsyncTasks();
            }
            else
            {
                SyncToAsync();
            }

        }

        private void SyncToAsync()
        {
            HttpClient httpClient = new HttpClient();

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44363/weatherforecast");

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

        public async Task AsyncCall()
        {
            HttpClient httpClient = new HttpClient();

            HttpRequestMessage httpRequestMessage =
                new HttpRequestMessage(HttpMethod.Get, "https://localhost:44363/weatherforecast");

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
    }
}