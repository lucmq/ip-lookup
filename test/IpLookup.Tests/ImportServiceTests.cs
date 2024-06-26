using System.Net;
using System.Text;
using IpLookup.Api.Downloader;
using IpLookup.Api.Import;
using IpLookup.Api.Lookup;
using IpLookup.Api.Storage.InMemory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace IpLookup.Api.Tests;

public class ImportServiceTests
{
    [Fact]
    public void Import_Adds_IpInfo_To_Index()
    {
        // Arrange
        var logger = NullLogger<ImportService>.Instance;
        var downloader = new Mock<Downloader.Downloader>();
        var importService = new ImportService(logger, downloader.Object);
        var indexMock = new Mock<IIpIndex>();
        var csvData = "192.168.0.1,192.168.0.10,US,New York,,New York City,,40.7128,-74.0060,";

        // Act
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));
        using var reader = new StreamReader(stream);
        importService.Import(reader, indexMock.Object);

        // Assert
        indexMock.Verify(x =>
                             x.Add(It.IsAny<IPAddress>(), It.IsAny<IPAddress>(), It.IsAny<IpInfo>()), Times.Once);
    }

    [Fact]
    public void GetFileReader_Returns_StreamReader_For_Remote_File()
    {
        // Arrange
        var logger = NullLogger<ImportService>.Instance;
        var httpClientFactoryMock = TestData.DbIpCityIpv4RemoteMock();
        var downloader = new Mock<Downloader.Downloader>();
        var importService = new ImportService(logger, downloader.Object,
                                              httpClientFactoryMock.Object);
        var uri = new Uri("https://localhost/dbip-country-ipv4.csv");

        // Act
        var reader = importService.GetFileReader(uri);

        // Assert
        Assert.NotNull(reader);
    }

    [Fact]
    public void GetFileReader_WithRemoteGzippedFile_DownloadsAndStreamsFile()
    {
        // Arrange
        var httpClientFactoryMock = TestData.DbIpCityIpv4GzipRemoteMock();
        var mockDownloader = new Mock<IDownloader>();
        mockDownloader.Setup(d => d.Download(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<bool>()))
                      .ReturnsAsync(new DownloadResponse
                      {
                          FileUri = new Uri(TestData.DbIpCityIpv4Filepath)
                      });
        var importService = new ImportService(
            Mock.Of<ILogger<ImportService>>(),
            mockDownloader.Object,
            httpClientFactoryMock.Object)
        {
            DefaultDownloadDestination = Path.GetTempPath()
        };

        // Act
        var uri = new Uri("https://localhost/dbip-country-ipv4.csv.gz");
        var reader = importService.GetFileReader(uri);

        // Assert
        mockDownloader.Verify(
            downloader => downloader
                .Download(uri, importService.DefaultDownloadDestination, true),
            Times.Once);
        Assert.NotNull(reader);
    }

    [Fact]
    public void GetFileReader_Returns_StreamReader_For_Local_File()
    {
        // Arrange
        var logger = NullLogger<ImportService>.Instance;
        var downloader = new Mock<Downloader.Downloader>();
        var importService = new ImportService(logger, downloader.Object);
        var csvFilepath = TestData.DbIpCityIpv4Filepath;
        var uri = new Uri(csvFilepath);

        // Act
        var reader = importService.GetFileReader(uri);

        // Assert
        Assert.NotNull(reader);
    }

    [Fact]
    public void GetFileReader_Throws_Exception_For_Unsupported_Scheme()
    {
        // Arrange
        var logger = NullLogger<ImportService>.Instance;
        var downloader = new Mock<Downloader.Downloader>();
        var importService = new ImportService(logger, downloader.Object);
        var uri = new Uri("ftps://example.com/file.csv");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => importService.GetFileReader(uri));
    }
}