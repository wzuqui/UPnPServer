using UpnpServer.Configurations;
using UpnpServer.Services;

namespace UpnpServer.Workers;

internal class CloudflareWorker : BackgroundService
{
    private readonly ILogger<CloudflareWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly AppSettings _appSettings;

    public CloudflareWorker(ILogger<CloudflareWorker> pLogger
        , IServiceProvider pServiceProvider
        , AppSettings pAppSettings)
    {
        _logger = pLogger;
        _serviceProvider = pServiceProvider;
        _appSettings = pAppSettings;
    }

    protected override Task ExecuteAsync(CancellationToken pCancellationToken)
    {
        var xConfiguracoes = _appSettings.Cloudflare;

        if (!xConfiguracoes.Enabled)
        {
            _logger.LogInformation("Worker desativado");
            return Task.CompletedTask;
        }

        Task.Run(() => DoWork(pCancellationToken), pCancellationToken);

        return Task.CompletedTask;
    }

    private async Task DoWork(CancellationToken pCancellationToken)
    {
        while (!pCancellationToken.IsCancellationRequested)
        {
            try
            {
                using (var xScope = _serviceProvider.CreateScope())
                {
                    var xService = xScope.ServiceProvider.GetRequiredService<CloudflareService>();
                    await xService.DoWork();
                }

                var xRefreshIntervalSeconds = _appSettings.Cloudflare.RefreshIntervalSeconds;
                _logger.LogInformation("Aguardando {Interval} segundos para nova execução", xRefreshIntervalSeconds);
                Thread.Sleep(TimeSpan.FromSeconds(xRefreshIntervalSeconds));
            }
            catch (Exception xException)
            {
                _logger.LogError(xException, "{Mensagem}", xException.Message);
            }
        }
    }
}
