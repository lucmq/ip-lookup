using Microsoft.Extensions.Logging;
using Moq;

namespace IpLookup.Api.Tests;

public class DownloaderTests
{
    // Note: The HttpClients for the tests are mocked. If we want to use some real
    // servers, we can get sample data from a test URL like:
    //  - https://getsamplefiles.com/download/gzip/sample-1.gz

    [Fact]
    public async Task Download_WithDecompression_ShouldDownloadAndDecompressFile()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<Downloader.Downloader>>();
        var gzipRemoteMock = TestData.DbIpCityIpv4GzipRemoteMock();
        var downloader = new Downloader.Downloader(loggerMock.Object, gzipRemoteMock.Object);
        var sourceUri = new Uri("https://localhost/dbip-country-ipv4.csv");
        var destinationFolder = Path.GetTempPath();
        var decompress = true;

        // Act
        await downloader.Download(sourceUri, destinationFolder, decompress);

        // Assert
        var fileName = Path.GetFileNameWithoutExtension(sourceUri.LocalPath);
        var extractedFilePath = Path.Combine(destinationFolder, fileName);
        Assert.True(File.Exists(extractedFilePath), "Extracted file should exist");
    }

    [Fact]
    public async Task Download_WithoutDecompression_ShouldDownloadFile()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<Downloader.Downloader>>();
        var gzipRemoteMock = TestData.DbIpCityIpv4GzipRemoteMock();
        var downloader = new Downloader.Downloader(loggerMock.Object, gzipRemoteMock.Object);
        var sourceUri = new Uri("https://localhost/dbip-country-ipv4.csv");
        var destinationFolder = Path.GetTempPath();
        var decompress = false;

        // Act
        await downloader.Download(sourceUri, destinationFolder, decompress);

        // Assert
        var fileName = Path.GetFileName(sourceUri.LocalPath);
        var movedFilePath = Path.Combine(destinationFolder, fileName);
        Assert.True(File.Exists(movedFilePath), "Moved file should exist");
    }

    [Fact]
    public async Task Download_WithInvalidSourceUri_ShouldThrowException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<Downloader.Downloader>>();
        var downloader = new Downloader.Downloader(loggerMock.Object);
        var sourceUri = new Uri("/invalid_uri");
        var destinationFolder = Path.GetTempPath();
        var decompress = true;

        // Act & Assert
        await Assert.ThrowsAsync<NotSupportedException>(async () =>
                                                            await downloader.Download(
                                                                sourceUri, destinationFolder, decompress));
    }

    [Fact]
    public async Task Download_WithNonExistentSourceUri_ShouldThrowException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<Downloader.Downloader>>();
        var downloader = new Downloader.Downloader(loggerMock.Object);
        var sourceUri = new Uri("http://localhost/file.gz");
        var destinationFolder = Path.GetTempPath();
        var decompress = true;

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
                                                           await downloader.Download(
                                                               sourceUri, destinationFolder, decompress));
    }
}