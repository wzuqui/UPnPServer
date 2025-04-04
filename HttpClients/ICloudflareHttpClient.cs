using Refit;

namespace UpnpServer.HttpClients;

public interface ICloudflareHttpClient
{
    [Get("/client/v4/zones/{zoneId}/dns_records")]
    Task<GetDnsRecordsResponse> GetDnsRecords([Header("Authorization")] string bearerToken, string zoneId);

    [Patch("/client/v4/zones/{zoneId}/dns_records/{id}")]
    Task<HttpResponseMessage> PatchDnsRecord([Header("Authorization")] string bearerToken
        , string zoneId
        , string id
        , [Body] PatchDnsRecordRequest request);
}

public class GetDnsRecordsResponse
{
    public required List<RecordResponse> Result { get; set; }
}

public class PatchDnsRecordRequest
{
    public required string Content { get; set; }
    public required string Name { get; set; }
    public required bool Proxied { get; set; }
    public required int Ttl { get; set; }
    public required string Type { get; set; }
}

public class RecordResponse
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required string Content { get; set; }
    public bool Proxied { get; set; }
}
