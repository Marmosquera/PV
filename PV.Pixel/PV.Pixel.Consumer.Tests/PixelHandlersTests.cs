using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PV.Pixel.Consumer.Consumers;
using PV.Pixel.Messages;

namespace PV.Pixel.Consumer.Tests
{
    public class PixelHandlersTests
    {
        private IVisitLogger _visitLogger;
        private ILogger<PixelHandlers> _logger;


        private IPixelHandlers _sut;
        public PixelHandlersTests()
        {
            _visitLogger = Substitute.For<IVisitLogger>();
            _logger = Substitute.For<ILogger<PixelHandlers>>();
            _sut = new PixelHandlers(_visitLogger, _logger);
        }

        [Fact]
        public async Task PixelRequestedHandler_OK()
        {
            // Arrange
            PixelRequested? pixelRequested = new PixelRequested("R", "UA", "IP");

            //Act
            await _sut.PixelRequestedHandler(pixelRequested);

            //Assert
            await _visitLogger.Received().Append(Arg.Any<string>());

        }

        [Fact]
        public async Task PixelRequestedHandler_Empty_Message()
        {
            // Arrange
            PixelRequested? pixelRequested = null;

            //Act
            await _sut.PixelRequestedHandler(pixelRequested);

            //Assert
            await _visitLogger.DidNotReceive().Append(Arg.Any<string>());

        }


        [Fact]
        public async Task PixelRequestedHandler_Throw_Error()
        {
            // Arrange
            var exceptionMessage = "Exc Message";
            _visitLogger.Append(Arg.Any<string>()).ThrowsAsync(new Exception(exceptionMessage));

            PixelRequested? pixelRequested = new PixelRequested("R", "UA", "IP");

            //Act
            Func<Task> act = () => _sut.PixelRequestedHandler(pixelRequested);

            //Assert
            var exception = await Assert.ThrowsAnyAsync<Exception>(act);
            Assert.Equal(exceptionMessage, exception.Message);

        }
    }
}