using System.Net;
using IpLookup.Api.Lookup;
using IpLookup.Api.Storage.InMemory;
using Microsoft.Extensions.Logging;
using Moq;

namespace IpLookup.Api.Tests;

public class IpLookupServiceTests
{
    private readonly Mock<IIpIndex> _ipIndexMock = new();
    private readonly Mock<ILogger<IpLookupService>> _loggerMock = new();

    [Fact]
    public void Add_AddsNonBogonIpToIndex()
    {
        // Arrange
        var ipLookupService = new IpLookupService(_loggerMock.Object, _ipIndexMock.Object);
        var ipAddress = IPAddress.Parse("1.0.0.0");
        var ipInfo = new IpInfo("city", "region", "country");

        // Act
        ipLookupService.Add(ipAddress, ipAddress, ipInfo);

        // Assert
        _ipIndexMock.Verify(mock => mock.Add(ipAddress, ipAddress, ipInfo), Times.Once);
    }

    [Fact]
    public void Add_IgnoresBogonIp()
    {
        // Arrange
        var ipLookupService = new IpLookupService(_loggerMock.Object, _ipIndexMock.Object);
        var bogonIp = IPAddress.Parse("0.0.0.0");
        var ipInfo = new IpInfo("city", "region", "country");

        // Act
        ipLookupService.Add(bogonIp, bogonIp, ipInfo);

        // Assert
        _ipIndexMock.Verify(mock => mock.Add(bogonIp, bogonIp, ipInfo), Times.Never);
    }

    [Fact]
    public void TryGetValue_ReturnsTrueForMainIndex()
    {
        // Arrange
        var ipLookupService = new IpLookupService(_loggerMock.Object, _ipIndexMock.Object);
        var ipAddress = IPAddress.Parse("192.168.0.1");
        var ipInfo = new IpInfo("city", "region", "country");
        _ipIndexMock.Setup(mock => mock.TryGetValue(ipAddress, out ipInfo))
                    .Returns(true);

        // Act
        var result = ipLookupService.TryGetValue(ipAddress, out var resultIpInfo);

        // Assert
        Assert.True(result);
        Assert.Equal(ipInfo, resultIpInfo);
    }

    [Fact]
    public void TryGetValue_ReturnsTrueForBogonIndex()
    {
        // Arrange
        var ipLookupService = new IpLookupService(_loggerMock.Object, _ipIndexMock.Object);
        var bogonIp = IPAddress.Parse("0.0.0.0");
        var ipInfo = new IpInfo("city", "region", "country");
        _ipIndexMock.Setup(mock => mock.TryGetValue(bogonIp, out ipInfo))
                    .Returns(true);

        // Act
        var result = ipLookupService.TryGetValue(bogonIp, out var resultIpInfo);

        // Assert
        Assert.True(result);
        Assert.Equal(ipInfo, resultIpInfo);
    }

    [Fact]
    public void TryGetValue_ReturnsFalseForUnknownIp()
    {
        // Arrange
        var ipLookupService = new IpLookupService(_loggerMock.Object, _ipIndexMock.Object);
        var unknownIp = IPAddress.Parse("1.0.0.0");
        IpInfo ipInfo = default;
        _ipIndexMock.Setup(mock => mock.TryGetValue(unknownIp, out ipInfo))
                    .Returns(false);

        // Act
        var result = ipLookupService.TryGetValue(unknownIp, out _);

        // Assert
        Assert.False(result);
    }
}