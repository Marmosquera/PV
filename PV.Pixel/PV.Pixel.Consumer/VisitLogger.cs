using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace PV.Pixel.Consumer
{
    public interface IVisitLogger
    {
        Task Append(string logEntry);
    }

    public class VisitLogger : IVisitLogger
    {
        private readonly string logFilePath;
        private readonly ILogger<VisitLogger> _logger;

        public VisitLogger(
            IConfiguration configuration,
            ILogger<VisitLogger> logger)
        {
            _logger = logger;
            ArgumentNullException.ThrowIfNull(configuration["LogFile"]);
            logFilePath = Path.Combine(Directory.GetCurrentDirectory(), configuration["LogFile"]!);
            EnsureFolder();
        }

        private void EnsureFolder()
        {
            try
            {
                var logDirectory = Path.GetDirectoryName(logFilePath);
                if (!string.IsNullOrEmpty(logDirectory)) Directory.CreateDirectory(logDirectory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "logFilePath:{logFilePath}", logFilePath);
                throw;
            }
        }

        public async Task Append(string logEntry)
        {
            try
            {
                using StreamWriter streamwriter = new(logFilePath, true, Encoding.UTF8, 65536);
                await streamwriter.WriteLineAsync(logEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "logFilePath:{logFilePath}", logFilePath);
                throw;
            }
        }
    }
}
