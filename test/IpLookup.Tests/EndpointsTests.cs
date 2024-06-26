using System.Net;
using IpLookup.Api.Downloader;
using IpLookup.Api.Endpoints;
using IpLookup.Api.Import;
using IpLookup.Api.Lookup;
using IpLookup.Api.Storage.InMemory;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace IpLookup.Api.Tests;

public class EndpointsTests
{
    // Ip Lookup Endpoints

    [Fact]
    public void Lookup_ReturnsOk_WhenIpInfoIsFound()
    {
        // Arrange
        var lookupServiceMock = new Mock<IIpLookupService>();
        var ipInfo = new IpInfo();
        lookupServiceMock.Setup(x => x.TryGetValue(It.IsAny<IPAddress>(), out ipInfo))
                         .Returns(true);
        string address = "192.168.1.1";

        // Act
        var result = IpLookupEndpoints.Lookup(lookupServiceMock.Object, address);

        // Assert
        var okResult = Assert.IsType<Ok<IpInfo>>(result);
        Assert.Equal(ipInfo, okResult.Value);
    }

    [Fact]
    public void Lookup_ReturnsNotFound_WhenIpInfoIsNotFound()
    {
        // Arrange
        var lookupServiceMock = new Mock<IIpLookupService>();
        IpInfo ipInfo = default;
        lookupServiceMock.Setup(x => x.TryGetValue(It.IsAny<IPAddress>(), out ipInfo))
                         .Returns(false);
        string address = "192.168.1.1";

        // Act
        var result = IpLookupEndpoints.Lookup(lookupServiceMock.Object, address);

        // Assert
        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public void Lookup_Throws_WhenAddressIsInvalid()
    {
        // Arrange
        var lookupServiceMock = new Mock<IIpLookupService>();
        string address = "invalid_ip";

        // Act / Assert
        Assert.Throws<FormatException>(
            () => IpLookupEndpoints.Lookup(lookupServiceMock.Object, address));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Lookup_Throws_WhenAddressIsNullOrEmpty(string address)
    {
        // Arrange
        var lookupServiceMock = new Mock<IpLookupService>();

        // Act / Assert
        Assert.Throws<ArgumentException>(
            () => IpLookupEndpoints.Lookup(lookupServiceMock.Object, address));
    }

    // Stats Endpoints

    private static ImportService CreateImportService() =>
        new(new Mock<ILogger<ImportService>>().Object, new Mock<IDownloader>().Object);

    [Fact]
    public void Stats_ReturnsOkResult_WhenStatsAreFound()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ImportTask>>();
        var importService = CreateImportService();
        var ipIndexMock = new Mock<IIpIndex>();

        var options = Options.Create(
            new ImportTaskOptions { FileUri = TestData.DbIpCityIpv4Filepath });

        var importTask = new ImportTask(
            loggerMock.Object, importService, ipIndexMock.Object, options);

        // Act
        StatsEndpoints.GetStats(importTask, ipIndexMock.Object);
    }
}