using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace PV.Pixel.Consumer.Tests
{
    public class VisitLoggerTests : IDisposable
    {
        private IConfiguration _configuration;
        private ILogger<VisitLogger> _logger;
        private string _logfile = "log.txt";

        private IVisitLogger _sut;
        public VisitLoggerTests()
        {
            _configuration = Substitute.For<IConfiguration>();
            _configuration["LogFile"].Returns(_logfile);

            _logger = Substitute.For<ILogger<VisitLogger>>();
            _sut = new VisitLogger(_configuration, _logger);
        }

        public void Dispose()
        {
            File.Delete(_logfile);
        }

        [Fact]
        public void Ctor_Throw_Error()
        {
            // Arrange
            _configuration["LogFile"].ReturnsNull();

            //Act
            Action act = () => new VisitLogger(_configuration, _logger);

            //Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public async void Append_OK()
        {
            // Arrange
            var firstEntry = "first-entry";
            var logEntry = $"{DateTime.UtcNow}-entry";

            //Act
            await _sut.Append(firstEntry);
            await _sut.Append(logEntry);

            //Assert
            var logEntries = File.ReadAllLines(_logfile);
            Assert.NotEmpty(logEntries);
            Assert.Equal(logEntry, logEntries.Last());
        }


    }
}
