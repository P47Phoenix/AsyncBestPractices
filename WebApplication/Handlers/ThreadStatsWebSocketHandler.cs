using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Web;
using System.Web.WebSockets;
using Newtonsoft.Json;

namespace WebApplication.Handlers
{
    public class ThreadStatsWebSocketHandler : IHttpHandler 
    { 
        public void ProcessRequest(HttpContext context) 
        { 
            //Checks if the query is WebSocket request.  
            if (context.IsWebSocketRequest) 
            { 
                //If yes, we attach the asynchronous handler. 
                context.AcceptWebSocketRequest(WebSocketRequestHandler); 
            } 
        } 
 
        public bool IsReusable => false;

        //Asynchronous request handler. 
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
                        var stats = await Global.ThreadPoolLogger.SourceBlock.ReceiveAsync();

                        js.Serialize(jw, stats);

                        await jw.FlushAsync();

                        //Sends data back. 
                        await webSocket.SendAsync(new ArraySegment<byte>(ms.ToArray()),
                            WebSocketMessageType.Text, true, cancellationToken);

                        await Task.Delay(500);
                    }
                }
            
        } 
    } 
}