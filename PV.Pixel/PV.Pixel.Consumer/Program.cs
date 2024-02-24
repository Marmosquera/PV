// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PV.Pixel.Consumer;
using PV.Pixel.Consumer.Consumers;

var hostBuilder = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        builder.SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddTransient<IVisitLogger, VisitLogger>();
        services.AddTransient<IPixelHandlers, PixelHandlers>();
        services.AddHostedService<PixelConsumerBackgroundService>();
    });
await hostBuilder.RunConsoleAsync().ConfigureAwait(false);



