using System.Diagnostics;

namespace IpLookup.Api.Lookup;

/// <summary>
/// Represents information about a single IP address or a range of IPs.
/// </summary>
public readonly record struct IpInfo
{
    /// <summary>
    /// Stores generic information about an IP address. In general, it is used
    /// with bogon IP addresses, since other information such as city, region,
    /// or country do not make sense for them.
    /// </summary>
    public string Description { get; init; } = "";

    /// <summary>
    /// The city of the IP address.
    /// </summary>
    public string City => CityRegionCountry.City;

    /// <summary>
    /// A country administrative region of the IP address.
    /// </summary>
    public string Region => CityRegionCountry.Region;

    /// <summary>
    /// The country of the IP address.
    /// </summary>
    public string Country => CityRegionCountry.Country;

    /// <summary>
    /// The latitude of the IP address.
    /// </summary>
    public float Latitude { get; init; }

    /// <summary>
    /// The longitude of the IP address.
    /// </summary>
    public float Longitude { get; init; }

    // public string TimeZone { get; init; }
    // public string PostalCode { get; init; }

    internal CityRegionCountry CityRegionCountry { get; init; }

    /// <summary>
    /// Creates a new <see cref="IpInfo"/> instance when the city, region, and
    /// country are know.
    /// </summary>
    /// <param name="city">The city.</param>
    /// <param name="region">The administrative region.</param>
    /// <param name="country">The country.</param>
    public IpInfo(string city, string region, string country)
    {
        CityRegionCountry = new CityRegionCountry(city, region, country);
    }

    /// <summary>
    /// Creates a new <see cref="IpInfo"/> instance.
    /// </summary>
    /// <param name="description">A generic description for the IP or range.</param>
    public IpInfo(string description)
    {
        Description = description;
        CityRegionCountry = new CityRegionCountry("", "", "");
    }
}

internal readonly record struct CityRegionCountry
{
    private string Data { get; }
    private ushort CityLength { get; }
    private ushort RegionLength { get; }

    public string City => Data[..CityLength];
    public string Region => Data.Substring(CityLength, RegionLength);
    public string Country => Data[(CityLength + RegionLength)..];

    public CityRegionCountry(string city, string region, string country)
    {
        Debug.Assert(city.Length <= 256 * 256);
        Debug.Assert(region.Length <= 256 * 256);

        Data = $"{city}{region}{country}";

        CityLength = (ushort)city.Length;
        RegionLength = (ushort)region.Length;
    }
}