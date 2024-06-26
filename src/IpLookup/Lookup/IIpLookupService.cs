using System.Net;

namespace IpLookup.Api.Lookup;

/// <summary>
/// The interface for the IP lookup service.
/// </summary>
public interface IIpLookupService
{
    /// <summary>
    /// Adds the specified IP range along with the associated IP information to
    /// the IP index.
    /// </summary>
    /// <param name="rangeStart"> The first IP address in the range. </param>
    /// <param name="rangeEnd">  The last IP address in the range. </param>
    /// <param name="ipInfo">
    /// The information common for all IPs in the range.
    /// </param>
    void Add(IPAddress rangeStart, IPAddress rangeEnd, IpInfo ipInfo);

    /// <summary>
    /// Tries to retrieve the <see cref="IpInfo"/> associated with the
    /// specified <see cref="IPAddress"/>.
    /// </summary>
    /// <param name="ipAddress">
    /// The <see cref="IPAddress"/> to retrieve the <see cref="IpInfo"/> for.
    /// </param>
    /// <param name="ipInfo">
    /// When the method completes successfully, contains the <see cref="IpInfo"/>
    /// associated with the specified <see cref="IPAddress"/>.
    /// </param>
    /// <returns><c>true</c> if the <see cref="IpInfo"/> was found in the main
    /// index or the bogon index; otherwise, <c>false</c>.
    /// </returns>
    bool TryGetValue(IPAddress ipAddress, out IpInfo ipInfo);
}