using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;

namespace Cebv.core.domain;

public static class CebvClientHandler
{
    public static HttpClient SharedClient = new()
    {
        BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("dev_uri") ?? string.Empty),

        DefaultRequestHeaders =
        {
            Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
        }
    };
}