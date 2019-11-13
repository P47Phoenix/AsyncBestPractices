using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using WebApplication;

namespace CoreWebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddSingleton(provider => new Lazy<ThreadPoolLogger>(() =>
            {
                var threadPoolLogger = new ThreadPoolLogger();
                threadPoolLogger.Start();
                return threadPoolLogger;
            }));

            var rootUri = Configuration.GetValue<string>("RootUri") ?? throw new ArgumentNullException("RootUri");
            

            services.AddHttpClient("someapi", client => { client.BaseAddress = new Uri(rootUri); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(60),
                ReceiveBufferSize = 4 * 1024
            });

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/threadpool")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var threadPoolLogger = context.RequestServices.GetRequiredService<Lazy<ThreadPoolLogger>>().Value;
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await WebSocketRequestHandler(threadPoolLogger, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }

            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        
        //Asynchronous request handler. 
        public async Task WebSocketRequestHandler(ThreadPoolLogger threadPoolLogger, WebSocket webSocket) 
        { 

            JsonSerializer js = new JsonSerializer();


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
                    var stats = await threadPoolLogger.SourceBlock.ReceiveAsync(cancellationToken);

                    stats.Error = threadPoolLogger.Error;

                    js.Serialize(jw, stats);

                    jw.Flush();

                    //Sends data back. 
                    await webSocket.SendAsync(new ArraySegment<byte>(ms.ToArray()),
                        WebSocketMessageType.Text, true, cancellationToken).ConfigureAwait(false);

                    await Task.Delay(100).ConfigureAwait(false);
                }
            }
            
        } 
    }
}
