using Microsoft.AspNetCore.Http.HttpResults;

namespace PV.Pixel.Service.Tests
{
    public class Gif1X1Tests
    {

        [Fact]
        public void ToResult_OK()
        {
            // Arrange
            var fileSize = 807;

            // Act
            var result = Gif1X1.ToResult();  //await _sut.ProducePixelRequested(pixelRequest);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<FileContentHttpResult>(result);
            var file = result as FileContentHttpResult;
            Assert.NotNull(file);
            Assert.Equal(Gif1X1.GIF_CONTENT_TYPE, file.ContentType);
            Assert.Equal(fileSize, file.FileLength);
        }
    }
}
