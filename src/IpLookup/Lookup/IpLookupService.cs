using System.Net;
using IpLookup.Api.Storage.InMemory;

namespace IpLookup.Api.Lookup;

/// <summary>
/// The IP lookup service is used to register and retrieve data about IPs in
/// the indexed IP ranges. It can also be used to check if an IP address is in
/// the <see href="https://ipinfo.io/bogon">bogon list</see>.
/// </summary>
/// <param name="logger"> The service logger. </param>
/// <param name="ipIndex"> The IP index to use. </param>
public class IpLookupService(ILogger<IpLookupService> logger, IIpIndex ipIndex)
    : IIpLookupService
{
    /// <inheritdoc/>
    public void Add(IPAddress rangeStart, IPAddress rangeEnd, IpInfo ipInfo)
    {
        if (IsBogonIp(rangeStart))
        {
            logger.LogWarning("Add: Bogon Ip address ignored, value: {value}",
                              rangeStart.ToString());
            return;
        }

        ipIndex.Add(rangeStart, rangeEnd, ipInfo);
    }

    /// <inheritdoc/>
    public bool TryGetValue(IPAddress ipAddress, out IpInfo ipInfo)
    {
        // If the IP address is found in the main index, return true
        // with the corresponding IpInfo. If not found in the main
        // index, check the bogon index.
        if (ipIndex.TryGetValue(ipAddress, out ipInfo))
            return true;

        return Ipv4BogonIndex.TryGetValue(ipAddress, out ipInfo);
    }

    /// <summary>
    /// Determines if the given IP address is a bogon IP.
    /// </summary>
    /// <param name="ipAddress">The IP address to check.</param>
    /// <returns>True if the IP address is a bogon IP, false otherwise.</returns>
    private static bool IsBogonIp(IPAddress ipAddress)
    {
        var ok = Ipv4BogonIndex.TryGetValue(ipAddress, out _);
        return ok;
    }
}