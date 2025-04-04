using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace UpnpServer.Workers;

internal class MainWorker : IHostedService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MainWorker> _logger;

    public MainWorker(ILogger<MainWorker> pLogger
        , IConfiguration pConfiguration)
    {
        _logger = pLogger;
        _configuration = pConfiguration;
    }

    public Task StartAsync(CancellationToken pCancellationToken)
    {
        _logger.LogInformation("------------------------------------------------------------------------------------------------------------------------");
        _logger.LogInformation("Iniciando serviço");
        _logger.LogInformation(new AssemblyInfo());

        foreach (var xKeyValuePair in _configuration.AsEnumerable())
        {
            _logger.LogInformation("Configuração...: {Key} = {Value}"
                , xKeyValuePair.Key
                , xKeyValuePair.Value);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken pCancellationToken)
    {
        _logger.LogInformation("Parando worker");
        return Task.CompletedTask;
    }
}

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public record AssemblyInfo
{
    public string ProcessName { get; } = Process.GetCurrentProcess().ProcessName;
    public string BaseDirectory { get; } = AppContext.BaseDirectory;
    public Version? Version { get; } = Assembly.GetEntryAssembly()?.GetName().Version;
    public DateTime LastWriteTime { get; } = File.GetLastWriteTime(AppContext.BaseDirectory);

    public void LogInformation(ILogger pLogger)
    {
        pLogger.LogInformation("ProcessName.....: {ProcessName}", ProcessName);
        pLogger.LogInformation("BaseDirectory...: {BaseDirectory}", BaseDirectory);
        pLogger.LogInformation("Version.........: {Version}", Version);
        pLogger.LogInformation("LastWriteTime...: {LastWriteTime:O}", LastWriteTime);
    }
}

public static class AssemblyInfoExtension
{
    public static void LogInformation(this ILogger pLogger
        , AssemblyInfo pAssemblyInfo)
    {
        pAssemblyInfo.LogInformation(pLogger);
    }
}
