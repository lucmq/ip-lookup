using System.Net;
using IpLookup.Api.Lookup;

namespace IpLookup.Api.Storage.InMemory;

/// <summary>
/// The IP index is used to store and retrieve <see cref="IpInfo"/> objects
/// for specific IP ranges.
/// </summary>
public interface IIpIndex
{
    /// <summary>
    /// Gets the number of <see cref="IpInfo"/> objects in the index.
    /// </summary>
    long InfoCount { get; }

    /// <summary>
    /// Gets the number of IP ranges in the index.
    /// </summary>
    long RangeCount { get; }

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
    bool TryGetValue(IPAddress ip, out IpInfo value);

    /// <summary>
    /// Adds the specified IP range along with the associated IP information to
    /// the index.
    /// </summary>
    /// <param name="rangeStart">The first IP address in the range.</param>
    /// <param name="rangeEnd">The last IP address in the range.</param>
    /// <param name="ipInfo">The information common for all IPs in the range.</param>
    void Add(IPAddress rangeStart, IPAddress rangeEnd, IpInfo ipInfo);
}