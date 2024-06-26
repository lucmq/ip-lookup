using System.Net;
using IpLookup.Api.Import;
using IpLookup.Api.Lookup;
using Microsoft.Extensions.Logging.Abstractions;

namespace IpLookup.Api.Tests.IpIndex;

public class IpIndexTests
{
    private static readonly PopulatedIpIndex IpIndex = new();

    private class PopulatedIpIndex : Storage.InMemory.IpIndex
    {
        private static readonly Downloader.Downloader Downloader =
            new(NullLogger<Downloader.Downloader>.Instance);

        private readonly ImportService _importService =
            new(NullLogger<ImportService>.Instance, Downloader);

        public PopulatedIpIndex()
        {
            var uri = new Uri(TestData.DbIpCityIpv4Filepath);
            _importService.Import(uri, this);
        }
    }

    [Fact]
    public void Add_AddsIpRange()
    {
        // Arrange
        var ipIndex = IpIndex;

        // Act
        // Already done since the IpIndex was populated at startup.

        // Assert
        Assert.Equal(200, ipIndex.RangeCount);
    }

    public static IEnumerable<object[]> GetExactIpData()
    {
        var data = new[]
        {
            ("1.0.0.0", "AU,Queensland,South Brisbane,-27.4767,153.017"),
            ("1.0.17.0", "JP,Tokyo,Shinjuku (1-chome),35.6944,139.703"),
            ("1.0.169.0", "TH,Bangkok,Khwaeng Thung Song Hong,13.8941,100.584"),
            ("1.1.1.0", "AU,New South Wales,Sydney,-33.8688,151.209"),
            ("1.1.172.0", "TH,Chiang Rai,Phan,19.5539,99.7404"),
            ("1.2.3.0", "AU,Queensland,South Brisbane,-27.4767,153.017"),
            ("1.4.4.0", "CN,Beijing,Beijing,39.9042,116.407"),
            ("1.4.203.0", "TH,Chon Buri,Pattaya,12.9333,100.883")
        };
        foreach (var element in data)
        {
            var (ip, infoString) = element;
            yield return [IPAddress.Parse(ip), ParseInfoString(infoString)];
        }
    }

    [Theory]
    [MemberData(nameof(GetExactIpData))]
    public void Get_ReturnsIpInfo(IPAddress ipAddress, IpInfo expected)
    {
        // Arrange
        var ipIndex = IpIndex;

        // Act
        var ok = ipIndex.TryGetValue(ipAddress, out var result);

        // Assert
        Assert.True(ok, $"IpInfo not found for {ipAddress}");
        Assert.Equal(expected.City, result.City);
        Assert.Equal(expected.Region, result.Region);
        Assert.Equal(expected.Country, result.Country);
        Assert.Equal(expected.Latitude, result.Latitude);
        Assert.Equal(expected.Longitude, result.Longitude);
    }

    public static IEnumerable<object[]> GetInRangeData()
    {
        var data = new[]
        {
            ("1.0.0.0", "AU,Queensland,South Brisbane,-27.4767,153.017"),
            ("1.0.0.1", "AU,Queensland,South Brisbane,-27.4767,153.017"),
            ("1.0.0.255", "AU,Queensland,South Brisbane,-27.4767,153.017"),
            ("1.0.17.0", "JP,Tokyo,Shinjuku (1-chome),35.6944,139.703"),
            ("1.0.17.1", "JP,Tokyo,Shinjuku (1-chome),35.6944,139.703"),
            ("1.0.30.1", "JP,Tokyo,Shinjuku (1-chome),35.6944,139.703"),
            ("1.0.31.255", "JP,Tokyo,Shinjuku (1-chome),35.6944,139.703"),
            ("1.0.173.0", "TH,Bangkok,Khwaeng Thung Song Hong,13.8941,100.584"),
            ("1.1.1.10", "AU,New South Wales,Sydney,-33.8688,151.209"),
            ("1.1.172.255", "TH,Chiang Rai,Phan,19.5539,99.7404"),
            ("1.2.3.255", "AU,Queensland,South Brisbane,-27.4767,153.017"),
            ("1.4.4.255", "CN,Beijing,Beijing,39.9042,116.407"),
            ("1.4.202.255", "TH,Bangkok,Khwaeng Thung Song Hong,13.8941,100.584")
        };
        foreach (var element in data)
        {
            var (ip, infoString) = element;
            yield return [IPAddress.Parse(ip), ParseInfoString(infoString)];
        }
    }

    [Theory]
    [MemberData(nameof(GetInRangeData))]
    public void Get_ReturnsInfoRecordForPreviousIp_WhenIpIsBetweenTwoIndexedIps(
        IPAddress ipAddress, IpInfo expected)
    {
        // Arrange
        var ipIndex = IpIndex;

        // Act
        var ok = ipIndex.TryGetValue(ipAddress, out var result);

        // Assert
        Assert.True(ok, $"IpInfo not found for {ipAddress}");
        Assert.Equal(expected.City, result.City);
        Assert.Equal(expected.Region, result.Region);
        Assert.Equal(expected.Country, result.Country);
        Assert.Equal(expected.Latitude, result.Latitude);
        Assert.Equal(expected.Longitude, result.Longitude);
    }

    [Fact]
    public void Add_ThrowsException_WhenIpRangesNotSorted()
    {
        // Arrange
        var ipIndex = IpIndex;
        var ipInfo = new IpInfo("South Brisbane", "Queensland", "AU")
        {
            Latitude = -27.4767f, Longitude = 153.017f
        };
        var ipAddress = IPAddress.Parse("1.0.0.0");

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
                                                              ipIndex.Add(ipAddress, ipAddress, ipInfo));
        Assert.Equal("IP ranges must be sorted in ascending order", ex.Message);
    }

    [Fact]
    public void Add_ThrowsException_WhenIpRangeIsNotValid()
    {
        // Arrange
        var ipIndex = new Storage.InMemory.IpIndex();
        var ipInfo = new IpInfo("South Brisbane", "Queensland", "AU")
        {
            Latitude = -27.4767f, Longitude = 153.017f
        };
        var startIp = IPAddress.Parse("1.0.0.1");
        var endIp = IPAddress.Parse("1.0.0.0");

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
                                                      ipIndex.Add(startIp, endIp, ipInfo));
        Assert.Equal("The IP range end must be greater than the start", ex.Message);
    }

    [Theory]
    [InlineData("0.1.0.0")]
    [InlineData("255.255.255.255")]
    public void Get_ThrowsException_WhenIpOutOfBounds(string ip)
    {
        // Arrange
        var ipIndex = IpIndex;
        var ipAddress = IPAddress.Parse(ip);

        // Act
        var ok = ipIndex.TryGetValue(ipAddress, out _);

        // Assert
        Assert.False(ok, "IpInfo retrieval should fail for out-of-bounds IP");
    }

    private static IpInfo ParseInfoString(string element)
    {
        var parts = element.Split(',');
        return new IpInfo(parts[2], parts[1], parts[0])
        {
            Latitude = Convert.ToSingle(parts[3]),
            Longitude = Convert.ToSingle(parts[4])
        };
    }
}