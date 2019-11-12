using System;
using System.Net.Http;

namespace WebApplication
{
    public interface IHttpClientFactory
    {
        HttpClient Create(Uri rootUri);
    }
}