using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRequest
{
    using Newtonsoft.Json;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Serilog.Context;

namespace CarDashboard.Pages.CRM.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class WebRequestHelper
    {
        
        /// <summary>
        /// Uses Cookies from cardashboard to post to an api for the purpose of authentication.
        /// You should never have to use this. If you do, you have have technical debt that needs to be put into the backlog.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="relativeToServerRootUri">The relative URI.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static TResponse Post<TResponse, TRequest>(Uri uri, TRequest body) 
            where TRequest : class
            where TResponse : class
        {
            var stopwatch = Stopwatch.StartNew();
            var httpRequest = WebRequest.CreateHttp(uri);
            using (LogContext.PushProperty($"action", nameof(Post)))
            using (LogContext.PushProperty($"CookieAuthPost.{nameof(httpRequest.RequestUri)}", httpRequest.RequestUri))
            using(MemoryStream requestBuffer = new MemoryStream())
            using (var streamWriter = new StreamWriter(requestBuffer, Encoding.UTF8))
            using (var requestJsonStream = new JsonTextWriter(streamWriter))
            {
                var js = new JsonSerializer();
                try
                {
                    Log.Debug("{action} to {CookieAuthPost.RequestUri}");


                    httpRequest.ContentType = "application/json";

                    httpRequest.Method = "POST";

                    httpRequest.Headers["FRONT-END-HTTPS"] = "ON";
                    
                    // if you can't return in less then 500 ms
                    // fix your api so it can. 
                    // and to be blunt 500 ms is probably to much time 
                    httpRequest.Timeout = 500;

                    js.Serialize(requestJsonStream, body);

                    requestJsonStream.Flush();

                    httpRequest.ContentLength = requestBuffer.Length;

                    using (var requestStream = httpRequest.GetRequestStream())
                    {
                        requestBuffer.Position = 0;
                        requestBuffer.CopyTo(requestStream);

                        Log.Debug($"Making an api call");
                        using (var httpWebResponse = (HttpWebResponse)httpRequest.GetResponse())
                        {
                            var statusCode = httpWebResponse.StatusCode;
                            Log.Debug("Web request completed successfully statusCode={statusCode}", statusCode);
                            using (var responseStream = httpWebResponse.GetResponseStream())
                                
                            using (var streamReader = new StreamReader(responseStream, Encoding.UTF8))
                            using (var responseJsonStream = new JsonTextReader(streamReader))
                            {
                                return js.Deserialize<TResponse>(responseJsonStream);
                            }
                        }
                    }
                    
                }
                catch (NotSupportedException notSupportedException)
                {
                    Log.Error(notSupportedException, "Error making web request");
                }
                catch (ProtocolViolationException protocolViolationException)
                {
                    Log.Error(protocolViolationException, "Error making web request");
                }
                catch (WebException error)
                {
                    var status = error.Status;
                    Log.Error(error, "Error making web request Status={status}", status);
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    Log.Error(invalidOperationException, "Error making web request");
                }
                finally
                {
                    stopwatch.Stop();
                    Log.Debug($"Api call completed");
                }
            }

            return null;
        }
    }
}
}
