using Microsoft.Extensions.Configuration;
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
        public VisitLogger(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration["LogFile"]);
            logFilePath = Path.Combine(Directory.GetCurrentDirectory(), configuration["LogFile"]!);
            EnsureFolder();
        }

        private void EnsureFolder()
        {
            var logDirectory = Path.GetDirectoryName(logFilePath);
            if (!string.IsNullOrEmpty(logDirectory)) Directory.CreateDirectory(logDirectory);
        }

        public async Task Append(string logEntry)
        {
            using StreamWriter streamwriter = new(logFilePath, true, Encoding.UTF8, 65536);
            await streamwriter.WriteLineAsync(logEntry);
        }
    }
}
