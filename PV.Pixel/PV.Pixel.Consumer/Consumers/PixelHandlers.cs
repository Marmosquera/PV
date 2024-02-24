using Microsoft.Extensions.Logging;
using PV.Pixel.Messages;

namespace PV.Pixel.Consumer.Consumers
{
    public interface IPixelHandlers
    {
        Task PixelRequestedHandler(PixelRequested? pixelRequested);
    }

    public class PixelHandlers : IPixelHandlers
    {
        private readonly IVisitLogger _visitLogger;
        private readonly ILogger<PixelHandlers> _logger;

        public PixelHandlers(
            IVisitLogger visitLogger,
            ILogger<PixelHandlers> logger)
        {
            _visitLogger = visitLogger;
            _logger = logger;
        }

        public async Task PixelRequestedHandler(PixelRequested? pixelRequested)
        {
            try
            {
                if (pixelRequested == null) return;
                var logEntry = $"{pixelRequested.RequestedOn:o}|{pixelRequested.Referer ?? "null"}|{pixelRequested.UserAgent ?? "null"}|{pixelRequested.VisitorIp}";
                Console.WriteLine($"log:{logEntry}");
                await _visitLogger.Append(logEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "pixelRequested:{pixelRequested}", pixelRequested);
                throw;
            }
        }
    }
}
