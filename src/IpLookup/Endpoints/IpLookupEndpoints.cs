using System.ComponentModel.DataAnnotations;
using System.Net;
using IpLookup.Api.Lookup;

namespace IpLookup.Api.Endpoints;

/// <summary>
/// The IP lookup endpoints.
/// </summary>
public static class IpLookupEndpoints
{
    /// <summary>
    /// Maps the IP lookup endpoints.
    /// </summary>
    /// <param name="app">The web application.</param>
    public static void MapIpLookupEndpoints(this WebApplication app)
    {
        app.MapGet("/lookup/{address}", Lookup)
           .WithName("LookupIp")
           .WithDescription("Looks up the information for the given IP address.")
           .WithOpenApi();
    }

    /// <summary>
    /// Looks up the information for the given IP address.
    /// </summary>
    /// <param name="lookupService">The IP lookup service.</param>
    /// <param name="address">The IP address to lookup.</param>
    /// <returns></returns>
    public static IResult Lookup(IIpLookupService lookupService,
                                 [Required] string address)
    {
        var ip = IPAddress.Parse(address);
        var ok = lookupService.TryGetValue(ip, out var ipInfo);
        return ok
            ? Results.Ok(ipInfo)
            : Results.NotFound();
    }
}