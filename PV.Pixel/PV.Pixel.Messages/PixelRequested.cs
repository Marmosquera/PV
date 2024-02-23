namespace PV.Pixel.Messages
{
    public record PixelRequested(string? Referer, string? UserAgent, string? VisitorIp)
    {
        public DateTime RequestedOn { get; set; } = DateTime.UtcNow;

    }
}
