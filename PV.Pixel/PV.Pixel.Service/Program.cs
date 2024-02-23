using Microsoft.AspNetCore.Mvc;
using PV.Pixel.Messages;
using PV.Pixel.Service;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IPixelProducer, PixelProducer>();

var app = builder.Build();

app.UseHttpsRedirection();


app.MapGet("/track", async ([FromServices] IPixelProducer pixelProducer, HttpRequest request) =>
{
    var pixelRequested = new PixelRequested(request.Headers?.Referer, request.Headers?.UserAgent, request.HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4()?.ToString());
    await pixelProducer.ProducePixelRequested(pixelRequested);
    return Gif1X1.ToResult();
});

app.Run();

