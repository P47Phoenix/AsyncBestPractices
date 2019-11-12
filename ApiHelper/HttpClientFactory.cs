using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;

namespace WebApplication
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private static ConcurrentDictionary<Uri, Lazy<HttpClient>> _concurrentDictionary = new ConcurrentDictionary<Uri, Lazy<HttpClient>>();
        public HttpClient Create(Uri rootUri)
        {
            return _concurrentDictionary.GetOrAdd(rootUri, uri => new Lazy<HttpClient>(() =>
            {
                var oneMinute = (int) TimeSpan.FromMinutes(1).TotalMilliseconds;
                var servicepoint = ServicePointManager.FindServicePoint(uri);
                servicepoint.ConnectionLeaseTimeout = oneMinute;
                servicepoint.ConnectionLimit = 100;
                ServicePointManager.DnsRefreshTimeout = oneMinute;
                return new HttpClient
                {
                    BaseAddress = uri,
                    //Timeout = TimeSpan.FromMilliseconds(500)
                };
            })).Value;
        }
    }
}