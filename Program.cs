using Microsoft.Extensions.Options;
using NLog.Web;
using Refit;
using UpnpServer.Configurations;
using UpnpServer.HttpClients;
using UpnpServer.Servers;
using UpnpServer.Services;
using UpnpServer.Workers;

var xBuilder = WebApplication.CreateBuilder(args);
{
    xBuilder.Logging.ClearProviders();
    xBuilder.Logging.AddConsole();
    xBuilder.Host.UseNLog();

    // configurations
    xBuilder.Configuration.AddUserSecrets<Program>(optional: true);
    xBuilder.Services.Configure<AppSettings>(xBuilder.Configuration);
    xBuilder.Services.AddSingleton(p => p.GetRequiredService<IOptions<AppSettings>>().Value);

    // contexts

    // automapper

    // repositories

    // services
    xBuilder.Services.AddMemoryCache();
    xBuilder.Services.AddScoped<CloudflareService>();

    // http clients
    xBuilder.Services.AddRefitClient<IIpIfyHttpClient>()
        .ConfigureHttpClient(p => p.BaseAddress = new Uri("https://api.ipify.org"));
    xBuilder.Services.AddRefitClient<ICloudflareHttpClient>()
        .ConfigureHttpClient((p, pS) => pS.BaseAddress = new Uri(p.GetRequiredService<AppSettings>().Cloudflare.BaseUrl));

    // workers
    xBuilder.Services.AddHostedService<MainWorker>();
    xBuilder.Services.AddHostedService<CloudflareWorker>();
    xBuilder.Services.AddHostedService<UPnPServer>();

    // swagger
    xBuilder.Services.AddEndpointsApiExplorer();
    xBuilder.Services.AddSwaggerGen();

    // controllers
    xBuilder.Services.AddControllers();
}

var xApp = xBuilder.Build();
{
    // swagger
    xApp.UseSwagger();
    xApp.UseSwaggerUI();

    // controllers
    xApp.MapControllers();
    xApp.MapGet("/", () => "Ok");
}

xApp.Run();
