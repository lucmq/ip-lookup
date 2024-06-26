using System.Net;
using IpLookup.Api.Lookup;
using IpLookup.Api.Storage.InMemory.DataStructures;
using static IpLookup.Api.Utilities.IpConverter;

namespace IpLookup.Api.Storage.InMemory;

/// <summary>
/// The IP index is used to store and retrieve <see cref="IpInfo"/> objects
/// for specific IP ranges.
/// </summary>
public class IpIndex : IIpIndex
{
    private const int IpCountHint = 4000000;
    private const int DistinctInfoCountHint = 400000;

    private readonly IntervalList<ulong, int> _infoKeyByIpRange =
        new(IpCountHint);

    private readonly MemoryColumn<InfoRecord> _ipInfoRecords =
        new(DistinctInfoCountHint);

    /// <inheritdoc />
    public long InfoCount => _ipInfoRecords.Count;

    /// <inheritdoc />
    public long RangeCount => _infoKeyByIpRange.Count;

    private ulong? _lastIp;

    /// <inheritdoc />
    public void Add(IPAddress rangeStart, IPAddress rangeEnd, IpInfo ipInfo)
    {
        var startIp = IpAddressToUInt64(rangeStart);
        var endIp = IpAddressToUInt64(rangeEnd);
        ValidateAddRequest(startIp, endIp);

        var infoRecord = new InfoRecord(ipInfo);
        var key = _ipInfoRecords.Add(infoRecord);

        _infoKeyByIpRange.Add(startIp, endIp, key);

        _lastIp = endIp;
    }

    /// <inheritdoc />
    public bool TryGetValue(IPAddress ip, out IpInfo value)
    {
        var ipAddressInt = IpAddressToUInt64(ip);

        var ok = _infoKeyByIpRange.TryGetValue(ipAddressInt, out var key);
        if (!ok)
        {
            value = default;
            return false;
        }

        var record = _ipInfoRecords.Get(key);
        value = record.IpInfo();
        return true;
    }

    private void ValidateAddRequest(ulong startIp, ulong endIp)
    {
        if (startIp <= _lastIp.GetValueOrDefault())
        {
            const string msg = "IP ranges must be sorted in ascending order";
            throw new InvalidOperationException(msg);
        }

        if (startIp > endIp)
        {
            const string msg = "The IP range end must be greater than the start";
            throw new ArgumentException(msg);
        }
    }

    private readonly record struct InfoRecord
    {
        private CityRegionCountry CityRegionCountry { get; }
        private float Latitude { get; }
        private float Longitude { get; }

        /// <summary>
        /// Creates a new <see cref="InfoRecord"/> instance.
        /// </summary>
        /// <param name="ip">The IP information.</param>
        public InfoRecord(IpInfo ip)
        {
            CityRegionCountry = ip.CityRegionCountry;
            Latitude = ip.Latitude;
            Longitude = ip.Longitude;
        }

        /// <summary>
        /// Gets the <see cref="IpInfo"/> associated with the <see cref="InfoRecord"/>.
        /// </summary>
        /// <returns>The <see cref="IpInfo"/>.</returns>
        public IpInfo IpInfo()
        {
            return new IpInfo
            {
                CityRegionCountry = CityRegionCountry,
                Latitude = Latitude,
                Longitude = Longitude
            };
        }
    }
}