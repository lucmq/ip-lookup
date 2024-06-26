using System.Net;
using IpLookup.Api.Lookup;

namespace IpLookup.Api.Tests.IpIndex;

public class IpIndexUnitTests
{
    [Fact]
    public void Add_AddsIpRange()
    {
        // Arrange
        var ipIndex = new Storage.InMemory.IpIndex();
        var ipInfo = new IpInfo("city", "region", "country")
        {
            Latitude = 1.0f, Longitude = 2.0f
        };
        var ipAddress = IPAddress.Parse("192.168.1.0");

        // Act
        ipIndex.Add(ipAddress, ipAddress, ipInfo);

        // Assert
        Assert.Equal(1, ipIndex.InfoCount);
        Assert.Equal(1, ipIndex.RangeCount);
    }

    [Fact]
    public void Add_AddsIpv6Range()
    {
        // Arrange
        var ipIndex = new Storage.InMemory.IpIndex();
        var ipInfo = new IpInfo("city", "region", "country")
        {
            Latitude = 1.0f, Longitude = 2.0f
        };
        var ipAddress = IPAddress.Parse("2001:0db8:85a3:0000:0000:8a2e:0370:7334");

        // Act
        ipIndex.Add(ipAddress, ipAddress, ipInfo);

        // Assert
        Assert.Equal(1, ipIndex.InfoCount);
        Assert.Equal(1, ipIndex.RangeCount);
    }

    [Fact]
    public void Get_ReturnsIpInfo()
    {
        // Arrange
        var ipIndex = new Storage.InMemory.IpIndex();
        var ipInfo = new IpInfo("city", "region", "country")
        {
            Latitude = 1.0f, Longitude = 2.0f
        };
        var ipAddress = IPAddress.Parse("192.168.1.0");
        ipIndex.Add(ipAddress, ipAddress, ipInfo);

        // Act
        var ok = ipIndex.TryGetValue(ipAddress, out var result);

        // Assert
        Assert.True(ok, $"IpInfo not found for {ipAddress}");
        Assert.Equal(ipInfo.City, result.City);
        Assert.Equal(ipInfo.Region, result.Region);
        Assert.Equal(ipInfo.Country, result.Country);
        Assert.Equal(ipInfo.Latitude, result.Latitude);
        Assert.Equal(ipInfo.Longitude, result.Longitude);
    }

    [Fact]
    public void Get_ReturnsInfoRecordForPreviousIp_WhenIpIsBetweenTwoIndexedIps()
    {
        // Arrange
        var ipIndex = new Storage.InMemory.IpIndex();
        var ipInfo1 = new IpInfo("city1", "region1", "country1")
        {
            Latitude = 1.0f, Longitude = 2.0f
        };
        var ipInfo2 = new IpInfo("city2", "region2", "country2")
        {
            Latitude = 1.0f, Longitude = 2.0f
        };
        var ipAddress1 = IPAddress.Parse("192.168.1.0");
        var ipAddress1End = IPAddress.Parse("192.168.2.255");
        var ipAddress2 = IPAddress.Parse("192.168.2.0");
        var ipAddress3 = IPAddress.Parse("192.168.3.0");
        ipIndex.Add(ipAddress1, ipAddress1End, ipInfo1);
        ipIndex.Add(ipAddress3, ipAddress3, ipInfo2);

        // Act
        var ok = ipIndex.TryGetValue(ipAddress2, out var result);

        // Assert
        Assert.True(ok, $"IpInfo not found for {ipAddress2}");
        Assert.Equal(ipInfo1.City, result.City);
        Assert.Equal(ipInfo1.Region, result.Region);
        Assert.Equal(ipInfo1.Country, result.Country);
        Assert.Equal(ipInfo1.Latitude, result.Latitude);
        Assert.Equal(ipInfo1.Longitude, result.Longitude);
    }

    [Fact]
    public void Get_ReturnsIpInfoForIpv6()
    {
        // Arrange
        var ipIndex = new Storage.InMemory.IpIndex();
        var ipInfo = new IpInfo("city", "region", "country")
        {
            Latitude = 1.0f, Longitude = 2.0f
        };
        var ipAddress = IPAddress.Parse("2001:0db8:85a3:0000:0000:8a2e:0370:7334");
        ipIndex.Add(ipAddress, ipAddress, ipInfo);

        // Act
        var ok = ipIndex.TryGetValue(ipAddress, out var result);

        // Assert
        Assert.True(ok, $"IpInfo not found for {ipAddress}");
        Assert.Equal(ipInfo.City, result.City);
        Assert.Equal(ipInfo.Region, result.Region);
        Assert.Equal(ipInfo.Country, result.Country);
        Assert.Equal(ipInfo.Latitude, result.Latitude);
        Assert.Equal(ipInfo.Longitude, result.Longitude);
    }

    [Fact]
    public void Add_ThrowsException_WhenIpRangesNotSorted()
    {
        // Arrange
        var ipIndex = new Storage.InMemory.IpIndex();
        var ipInfo = new IpInfo("city", "region", "country")
        {
            Latitude = 1.0f, Longitude = 2.0f
        };
        var ipAddress1 = IPAddress.Parse("192.168.1.0");
        var ipAddress2 = IPAddress.Parse("192.168.2.0");
        ipIndex.Add(ipAddress2, ipAddress2, ipInfo); // Add second IP range

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
                                                              ipIndex.Add(ipAddress1, ipAddress1, ipInfo));
        Assert.Equal("IP ranges must be sorted in ascending order", ex.Message);
    }

    [Fact]
    public void Get_ShouldFail_WhenIndexIsEmpty()
    {
        // Arrange
        var ipIndex = new Storage.InMemory.IpIndex();
        var ipAddress = IPAddress.Parse("192.168.1.0");

        // Act
        var ok = ipIndex.TryGetValue(ipAddress, out _);

        // Assert
        Assert.False(ok, "IpInfo retrieval should fail when the IpIndex is empty");
    }

    [Theory]
    [InlineData("192.168.0.0")]
    [InlineData("192.168.3.0")]
    public void Get_ShouldFail_WhenIpOutOfBounds(string ipToSearch)
    {
        // Arrange
        var ipIndex = new Storage.InMemory.IpIndex();
        var ipInfo = new IpInfo("city", "region", "country")
        {
            Latitude = 1.0f, Longitude = 2.0f
        };
        var ipAddress1 = IPAddress.Parse("192.168.1.0");
        var ipAddress2 = IPAddress.Parse("192.168.2.0");
        ipIndex.Add(ipAddress1, ipAddress1, ipInfo); // Add first IP range
        ipIndex.Add(ipAddress2, ipAddress2, ipInfo); // Add second IP range
        var ipAddressToSearch = IPAddress.Parse(ipToSearch);

        // Act
        var ok = ipIndex.TryGetValue(ipAddressToSearch, out _);

        // Assert
        Assert.False(ok, "IpInfo retrieval should fail for out-of-bounds IP");
    }
}