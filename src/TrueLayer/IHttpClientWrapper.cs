using System;
using System.Threading.Tasks;

namespace TrueLayer
{
    public interface IHttpClientWrapper
    {
        Uri BaseAddress { get; set; }
        Task<string> GetData(Uri endpoint, string token);
    }
}