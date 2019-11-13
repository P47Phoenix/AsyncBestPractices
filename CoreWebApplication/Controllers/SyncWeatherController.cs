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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using TestApiCall;
using WebApplication;

namespace CoreWebApplication.Controllers
{
    public class SyncWeatherController : Controller
    {
        private readonly Lazy<ThreadPoolLogger> _lazyThreadPoolLogger;
        private readonly IConfiguration _configuration;

        public SyncWeatherController(Lazy<ThreadPoolLogger> lazyThreadPoolLogger, IConfiguration configuration)
        {
            _lazyThreadPoolLogger = lazyThreadPoolLogger;
            _configuration = configuration;
        }
        
        public IActionResult Index()
        {
            var uri = new Uri($"{_configuration.GetValue<string>("RootUri")}weatherforecast");

            TcpClient client = new TcpClient();

            client.Connect(uri.Host, 443);

            using (SslStream sslStream = new SslStream(client.GetStream(), false, (sender, certificate, chain, errors) => true))
            {
                sslStream.AuthenticateAsClient(uri.Host);
                var response = WriteAndReadResponseStream(sslStream, outBoundWriter =>
                {
                    outBoundWriter.WriteLine("GET /SomeApi/weatherforecast HTTP/1.1");
                    outBoundWriter.WriteLine("Host: localhost");
                    outBoundWriter.WriteLine("Content-Type: Application/Json");
                    outBoundWriter.WriteLine("Connection: Close");
                    outBoundWriter.WriteLine("");
                    outBoundWriter.WriteLine("");
                    outBoundWriter.Flush();
                });

                var headersAndBody = response.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries);

                var body = headersAndBody[1];

                var splitBody = body.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

                if (splitBody.Length > 1)
                {
                    body = splitBody[1];
                }


                var data = JsonConvert.DeserializeObject<WeatherForecast[]>(body);
                //var data = new WeatherForecast[0];
                return View(data);
            }


        }

        public static string WriteAndReadResponseStream(Stream tcpStream, Action<StreamWriter> httpsRequest)
        {

            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter streamWriter = new StreamWriter(tcpStream, Encoding.ASCII, leaveOpen: true))
                {
                    httpsRequest(streamWriter);
                }

                byte[] inputBuffer = new byte[256];
                int length;
                do
                {
                    length = tcpStream.Read(inputBuffer, 0, inputBuffer.Length);

                    ms.Write(inputBuffer, 0, length);
                } while (length > 0);

                ms.Position = 0;

                using (StreamReader reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}