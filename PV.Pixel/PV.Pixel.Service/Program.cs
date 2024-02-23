var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/track", (HttpRequest request) =>
{
    var pixelRequest = new PixelRequest(request.Headers?.Referer, request.Headers?.UserAgent, request.HttpContext?.Connection?.RemoteIpAddress?.ToString());
    var response = $"tracked:{pixelRequest}";
    return response;
});

app.Run();

internal record PixelRequest(string? Referer, string? UserAgent, string? VisitorIp) { }