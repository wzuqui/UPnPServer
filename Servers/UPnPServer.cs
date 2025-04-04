using Microsoft.Extensions.Options;
using Open.Nat;
using UpnpServer.Configurations;

namespace UpnpServer.Servers;

internal class UPnPServer : IHostedService
{
    private readonly ILogger<UPnPServer> _logger;
    private readonly IOptionsMonitor<AppSettings> _appSettings;

    public UPnPServer(ILogger<UPnPServer> logger
        , IOptionsMonitor<AppSettings> appSettings)
    {
        _logger = logger;

        _appSettings = appSettings;
        _appSettings.OnChange(OnConfigChanged);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_appSettings.CurrentValue.UPnPServer.Enabled)
        {
            _logger.LogInformation("Worker desativado");
            return Task.CompletedTask;
        }

        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var pAppSettings = _appSettings.CurrentValue;

                await DoWork(pAppSettings);

                var xRefreshIntervalSeconds = pAppSettings.UPnPServer.RefreshIntervalSeconds;
                _logger.LogInformation("Aguardando {Interval} segundos para nova execução", xRefreshIntervalSeconds);
                await Task.Delay(TimeSpan.FromSeconds(xRefreshIntervalSeconds), cancellationToken);
            }
        }, cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Parando worker");

        return Task.CompletedTask;
    }

    private async Task DoWork(AppSettings pAppSettings)
    {
        var xDiscoverer = new NatDiscoverer();

        _logger.LogInformation("Procurando roteador UPnP...");

        var xDevice = await xDiscoverer.DiscoverDeviceAsync();
        var xExternalIp = await xDevice.GetExternalIPAsync();
        _logger.LogInformation("Endereço IP Externo: {ExternalIp}", xExternalIp);

        var xMappings = (await xDevice.GetAllMappingsAsync()).ToList();
        var xPorts = pAppSettings.UPnPServer.Ports;

        foreach (var xPort in xPorts)
        {
            try
            {
                var xExiste = xMappings
                    .Where(p => p.PrivatePort == xPort.PrivatePort)
                    .Where(p => p.PublicPort == xPort.PublicPort)
                    .Any(p => p.Protocol.ToString().Equals(xPort.Protocol, StringComparison.CurrentCultureIgnoreCase));

                if (xExiste)
                    _logger.LogInformation("Porta já mapeada: {@Port}", xPort);
                else
                {
                    var xProtocol = xPort.Protocol.Equals("TCP", StringComparison.CurrentCultureIgnoreCase)
                        ? Protocol.Tcp
                        : Protocol.Udp;
                    await xDevice.CreatePortMapAsync(new Mapping(xProtocol
                        , xPort.PrivatePort
                        , xPort.PublicPort
                        , xPort.Description
                    ));

                    _logger.LogInformation("Porta mapeada: {@Porta}", xPort);
                }
            }
            catch (Exception xException)
            {
                _logger.LogError(xException, "Erro ao mapear porta: {@Porta}", xPort);
            }
        }
    }

    private void OnConfigChanged(AppSettings pAppSettings)
    {
        _logger.LogInformation("Configuração alterada! Novo valor: {@AppSettings}", pAppSettings);
    }
}
