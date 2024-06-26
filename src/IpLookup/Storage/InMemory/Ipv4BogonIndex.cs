using System.Net;
using IpLookup.Api.Lookup;
using IpLookup.Api.Storage.InMemory.DataStructures;
using IpLookup.Api.Utilities;

namespace IpLookup.Api.Storage.InMemory;

/// <summary>
/// The IP index is used to store and retrieve <see cref="IpInfo"/> objects
/// for specific IP ranges. The bogon index specifically deals ranges in the
/// <see href="https://ipinfo.io/bogon">bogon list</see>.
/// </summary>
public static class Ipv4BogonIndex
{
    private static readonly IntervalList<ulong, IpInfo> IpInfoByInterval = new(20);

    private static readonly NetworkInfo[] BogonNetworks =
    {
        // Source: Bogon IP values (https://ipinfo.io/bogon)
        new("0.0.0.0", 8, "\"This\" network"),
        new("10.0.0.0", 8, "Private-use networks"),
        new("100.64.0.0", 10, "Carrier-grade NAT"),
        new("127.0.0.0", 8, "Loopback"),
        new("169.254.0.0", 16, "Link local"),
        new("172.16.0.0", 12, "Private-use networks"),
        new("192.0.0.0", 24, "IETF protocol assignments"),
        new("192.0.2.0", 24, "TEST-NET-1"),
        new("192.168.0.0", 16, "Private-use networks"),
        new("198.18.0.0", 15, "Network interconnect device benchmark testing"),
        new("198.51.100.0", 24, "TEST-NET-2"),
        new("203.0.113.0", 24, "TEST-NET-3"),
        new("224.0.0.0", 4, "Multicast"),
        new("240.0.0.0", 4, "Reserved for future use")
    };

    // Special cases:
    // new("127.0.53.53", 32, "Name collision occurrence"),
    // new("255.255.255.255", 32, "Limited broadcast")

    /// <summary>
    /// Gets the number of IP ranges in the index.
    /// </summary>
    public static int RangeCount => IpInfoByInterval.Count;

    static Ipv4BogonIndex()
    {
        Populate();
    }

    /// <summary>
    /// Tries to retrieve the <see cref="IpInfo"/> associated with the
    /// specified <see cref="IPAddress"/>.
    /// </summary>
    /// <param name="ip">The <see cref="IPAddress"/> to retrieve the
    /// <see cref="IpInfo"/> for.</param>
    /// <param name="value">When the method completes successfully, contains
    /// the <see cref="IpInfo"/> associated with the specified <see
    /// cref="IPAddress"/>.</param>
    /// <returns><c>true</c> if the <see cref="IpInfo"/> was found in the
    /// index; otherwise, <c>false</c>.</returns>
    public static bool TryGetValue(IPAddress ip, out IpInfo value)
    {
        var ipAddressInt = IpConverter.IpAddressToUInt64(ip);
        var ok = IpInfoByInterval.TryGetValue(ipAddressInt, out value);
        return ok;
    }

    private static void Populate()
    {
        foreach (var network in BogonNetworks)
        {
            var startIp = network.IpUInt64;
            var endIp = network.SubnettedIpUInt64;

            var ipInfo = new IpInfo(network.Description);

            IpInfoByInterval.Add(startIp, endIp, ipInfo);
        }
    }

    private readonly record struct NetworkInfo(
        string Address,
        byte SubnetMask,
        string Description
    )
    {
        public string Address { get; } = Address;
        public byte SubnetMask { get; } = SubnetMask;
        public string Description { get; } = Description;

        public ulong IpUInt64 =>
            IpConverter.IpAddressToUInt64(Address);

        public ulong SubnettedIpUInt64 =>
            IpConverter.SubnettedIp((uint)IpUInt64, SubnetMask);
    }
}