using Refit;

namespace UpnpServer.HttpClients;

public interface IIpIfyHttpClient
{
    [Get("/?format=json")]
    Task<GetIpIfyResponse> GetIpIfy();
}

public class GetIpIfyResponse
{
    public required string Ip { get; set; }
}
