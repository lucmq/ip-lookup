using IpLookup.Api.Downloader;
using IpLookup.Api.Import;
using IpLookup.Api.Storage.InMemory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace IpLookup.Api.Tests;

public class ImportTaskTests
{
    private static ImportService CreateImportService() =>
        new(new Mock<ILogger<ImportService>>().Object, new Mock<IDownloader>().Object);

    [Fact]
    public async Task StartAsync_ShouldImportDataAndLogInformation()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ImportTask>>();
        var importService = CreateImportService();
        var ipIndexMock = new Mock<IIpIndex>();

        var options = Options.Create(
            new ImportTaskOptions { FileUri = TestData.DbIpCityIpv4Filepath });

        var importTask = new ImportTask(
            loggerMock.Object, importService, ipIndexMock.Object, options);

        var cancellationToken = new CancellationToken();

        // Set up ipIndex properties
        ipIndexMock.SetupGet(x => x.RangeCount).Returns(10);
        ipIndexMock.SetupGet(x => x.InfoCount).Returns(20);

        // Act
        await importTask.StartAsync(cancellationToken);

        // Assert
        Assert.NotEqual(importTask.StartTime, DateTime.UnixEpoch);
        Assert.NotEqual(importTask.Duration, TimeSpan.Zero);
    }

    [Fact]
    public async Task StopAsync_CancellationToken_ReturnsCompletedTask()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ImportTask>>();
        var importService = CreateImportService();
        var ipIndexMock = new Mock<IIpIndex>();

        var options = Options.Create(
            new ImportTaskOptions { FileUri = TestData.DbIpCityIpv4Filepath });

        var importTask = new ImportTask(
            loggerMock.Object, importService, ipIndexMock.Object, options);

        var cancellationToken = new CancellationToken(true);

        // Act
        await importTask.StopAsync(cancellationToken);
    }
}