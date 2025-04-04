using UpnpServer.Configurations;
using UpnpServer.HttpClients;

namespace UpnpServer.Services;

internal class CloudflareService : IScoped
{
    private readonly ILogger<CloudflareService> _logger;
    private readonly AppSettings _appSettings;

    private readonly ICloudflareHttpClient _cloudflareHttpClient;
    private readonly IIpIfyHttpClient _ipIfyHttpClient;

    public CloudflareService(ILogger<CloudflareService> logger
        , AppSettings appSettings
        , ICloudflareHttpClient cloudflareHttpClient
        , IIpIfyHttpClient ipIfyHttpClient)
    {
        _logger = logger;
        _appSettings = appSettings;

        _cloudflareHttpClient = cloudflareHttpClient;
        _ipIfyHttpClient = ipIfyHttpClient;
    }

    public async Task DoWork()
    {
        try
        {
            var xGetIpIfyResponse = await _ipIfyHttpClient.GetIpIfy();
            var xPublicIp = xGetIpIfyResponse.Ip;

            _logger.LogInformation("IP atual: {Ip}", xPublicIp);

            var xGetDnsRecordsResponse = await _cloudflareHttpClient.GetDnsRecords(_appSettings.Cloudflare.BearerToken, _appSettings.Cloudflare.ZoneId);
            foreach (var xRecord in _appSettings.Cloudflare.Records)
            {
                var xPersistido = xGetDnsRecordsResponse.Result.FirstOrDefault(p => p.Name == xRecord.Name);

                if (xPersistido == null)
                {
                    _logger.LogInformation("Registro {Registro} não existe", xRecord.Name);
                    continue;
                }

                if (xPersistido.Content == xPublicIp)
                    _logger.LogInformation("Registro {Registro} já está atualizado", xRecord.Name);
                else
                {
                    var xHttpResponseMessage = await _cloudflareHttpClient.PatchDnsRecord(_appSettings.Cloudflare.BearerToken
                        , _appSettings.Cloudflare.ZoneId
                        , xPersistido.Id
                        , new PatchDnsRecordRequest
                        {
                            Name = xRecord.Name
                            , Content = xPublicIp
                            , Proxied = xRecord.Proxied
                            , Ttl = xRecord.TTL
                            , Type = xRecord.Type
                        });

                    if (xHttpResponseMessage.IsSuccessStatusCode)
                        _logger.LogInformation("Registro {Registro} atualizado com sucesso", xRecord.Name);
                }
            }
        }
        catch (Exception xException)
        {
            _logger.LogError(xException, "Erro ao atualizar o Cloudflare");
        }
    }
}
