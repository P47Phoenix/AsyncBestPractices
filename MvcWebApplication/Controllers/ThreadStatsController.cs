using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;
using Newtonsoft.Json;

namespace MvcWebApplication.Controllers
{
    public class ThreadStatsController : ApiController
    {
        public async Task<HttpResponseMessage> Get()
        {
            
            HttpContext.Current.AcceptWebSocketRequest(WebSocketRequestHandler);
            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
        }
        public async Task WebSocketRequestHandler(AspNetWebSocketContext webSocketContext) 
        { 

            JsonSerializer js = new JsonSerializer();

            //Gets the current WebSocket object. 
            WebSocket webSocket = webSocketContext.WebSocket;

            /*We define a certain constant which will represent 
            size of received data. It is established by us and  
            we can set any value. We know that in this case the size of the sent 
            data is very small. 
            */
            const int maxMessageSize = 1024;

            //Buffer for received bits. 
            var receivedDataBuffer = new ArraySegment<Byte>(new Byte[maxMessageSize]);

            var cancellationToken = new CancellationToken();

            //Checks WebSocket state. 
            while (webSocket.State == WebSocketState.Open)
            {
                using (MemoryStream ms = new MemoryStream())
                using (StreamWriter sm = new StreamWriter(ms))
                using (JsonWriter jw = new JsonTextWriter(sm))
                {
                    var stats = await WebApiApplication.ThreadPoolLogger.SourceBlock.ReceiveAsync(cancellationToken).ConfigureAwait(false);

                    stats.Error = WebApiApplication.ThreadPoolLogger.Error;

                    js.Serialize(jw, stats);

                    await jw.FlushAsync().ConfigureAwait(false);

                    //Sends data back. 
                    await webSocket.SendAsync(new ArraySegment<byte>(ms.ToArray()),
                        WebSocketMessageType.Text, true, cancellationToken).ConfigureAwait(false);

                    await Task.Delay(100).ConfigureAwait(false);
                }
            }
            
        } 
    }
}
