using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using static System.Buffers.Binary.BinaryPrimitives;

namespace IpLookup.Api.Utilities;

/// <summary>
/// Provides methods to convert between IP addresses and integers.
/// </summary>
public static class IpConverter
{
    /// <summary>
    /// Converts an IP address to a 64-bit unsigned integer.
    /// </summary>
    /// <param name="ipAddress">The IP address to convert.</param>
    /// <returns>The converted IP address as a 64-bit unsigned integer.</returns>
    public static ulong IpAddressToUInt64(IPAddress ipAddress)
    {
        const AddressFamily ipv4AddressFamily = AddressFamily.InterNetwork;
        Span<byte> bytes = stackalloc byte[16];

        var ok = ipAddress.TryWriteBytes(bytes, out var n);
        Debug.Assert(ok);
        Debug.Assert(n != 0);

        return ipAddress.AddressFamily switch
        {
            ipv4AddressFamily => ReadUInt32BigEndian(bytes),
            _ => ReadUInt64BigEndian(bytes)
        };
    }

    /// <summary>
    /// Converts an IP address represented as a string to a 64-bit unsigned integer.
    /// </summary>
    /// <param name="ip">The IP address as a string.</param>
    /// <returns>The converted IP address as a 64-bit unsigned integer.</returns>
    public static ulong IpAddressToUInt64(string ip)
    {
        var ipAddress = IPAddress.Parse(ip);
        return IpAddressToUInt64(ipAddress);
    }

    /// <summary>
    /// Calculates the subnetted IP address for a given IP address and subnet mask.
    /// </summary>
    /// <param name="ip">The IP address.</param>
    /// <param name="subnetMask">The subnet mask.</param>
    /// <returns>The subnetted IP address.</returns>
    public static uint SubnettedIp(uint ip, byte subnetMask)
    {
        var subnettedIp = ip + (uint)Math.Pow(2, 32 - subnetMask) - 1;
        return subnettedIp;
    }
}