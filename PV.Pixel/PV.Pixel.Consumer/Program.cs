// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PV.Pixel.Consumer;

var hostBuilder = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        builder.SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddTransient<IVisitLogger, VisitLogger>();
        services.AddHostedService<ConsumerBackgroundService>();
    });
await hostBuilder.RunConsoleAsync().ConfigureAwait(false);



