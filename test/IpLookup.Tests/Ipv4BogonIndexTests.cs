using System.Net;
using IpLookup.Api.Storage.InMemory;

namespace IpLookup.Api.Tests;

public class Ipv4BogonIndexTests
{
    [Fact]
    public void BogonIndex_Contains_Correct_Number_Of_IpInfo()
    {
        // Arrange
        var expectedCount = 14; // Number of entries in BogonNetworks array

        // Act
        var actualCount = Ipv4BogonIndex.RangeCount;

        // Assert
        Assert.Equal(expectedCount, actualCount);
    }

    [Theory]
    [InlineData("0.0.0.0")]
    [InlineData("0.0.0.1")]
    [InlineData("10.0.0.0")]
    [InlineData("10.0.1.0")]
    [InlineData("100.64.0.0")]
    [InlineData("127.0.0.0")]
    [InlineData("169.254.0.0")]
    [InlineData("172.16.0.0")]
    [InlineData("192.0.0.0")]
    [InlineData("192.0.2.0")]
    [InlineData("192.168.0.0")]
    [InlineData("198.18.0.0")]
    [InlineData("198.51.100.0")]
    [InlineData("203.0.113.0")]
    [InlineData("224.0.0.0")]
    [InlineData("240.0.0.0")]
    [InlineData("255.255.255.254")]
    [InlineData("255.255.255.255")]
    public void BogonIndex_TryGetValue_Returns_True_For_Known_Bogon_Ip(string ip)
    {
        // Arrange
        var knownBogonIp = IPAddress.Parse(ip);

        // Act
        var exists = Ipv4BogonIndex.TryGetValue(knownBogonIp, out var value);

        // Assert
        Assert.True(exists);
        Assert.NotEmpty(value.Description);
    }

    [Fact]
    public void BogonIndex_TryGetValue_Returns_False_For_Non_Bogon_Ip()
    {
        // Arrange
        var nonBogonIp = IPAddress.Parse("8.8.8.8");

        // Act
        var result = Ipv4BogonIndex.TryGetValue(nonBogonIp, out _);

        // Assert
        Assert.False(result);
    }
}