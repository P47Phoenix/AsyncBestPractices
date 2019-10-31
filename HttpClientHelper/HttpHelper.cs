using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.App_Code
{
    public class HttpHelper
    {
        private static HttpClient s_httpClient = new HttpClient();


        public static async Task<T> GetAsync<T>(string uri) where T : class, new()
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            HttpResponseMessage result = await S_httpClient.SendAsync(httpRequestMessage);


            if (result.IsSuccessStatusCode)
            {
                JsonSerializer js = new JsonSerializer();

                using (var resposeContent = await result.Content.ReadAsStreamAsync())
                {
                    return js.Deserialize<T>(resposeContent);
                }
            }

            return new T();
        }

        public static T GetSyncBad<T>(string uri) where T : class, new()
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            HttpResponseMessage result = S_httpClient.SendAsync(httpRequestMessage).Result;


            if (result.IsSuccessStatusCode)
            {
                JsonSerializer js = new JsonSerializer();

                using (var resposeContent = result.Content.ReadAsStreamAsync().Result)
                {
                    return js.Deserialize<T>(resposeContent);
                }
            }

            return new T();
        }

        public static T Get<T>(string uri) where T : class, new()
        {
            JsonSerializer js = new JsonSerializer();
            using (var request = new HttpWebRequest())
            {
                try
                {
                    request.RequestUri = uri;
                    request.Method = "GET";

                    using (var response = request.GetResponse())
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            return js.Deserialize<T>(stream);
                        }
                    }
                }
                catch (WebException webException)
                {
                }
            }

            return new T();
        }
    }
}