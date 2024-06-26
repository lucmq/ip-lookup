using IpLookup.Api.Import;
using IpLookup.Api.Storage.InMemory;

namespace IpLookup.Api.Endpoints;

/// <summary>
/// The endpoints for app information and statistics.
/// </summary>
public static class StatsEndpoints
{
    /// <summary>
    /// Maps the stats endpoints.
    /// </summary>
    /// <param name="app">The web application.</param>
    public static void MapStatsEndpoints(this WebApplication app)
    {
        app.MapGet("/", GetStats)
           .WithName("Home")
           .WithDescription("Returns general information and statistics for" +
                            "the webapp and its services.")
           .ExcludeFromDescription();
    }

    /// <summary>
    /// Gets general information and statistics for the webapp and its services.
    /// </summary>
    /// <param name="importTask">The app import task.</param>
    /// <param name="ipIndex">The IP index.</param>
    /// <returns></returns>
    public static IResult GetStats(ImportTask importTask, IIpIndex ipIndex)
    {
        return Results.Ok(new
        {
            // Show service information like time started, last data
            // import, indexed IPs count etc.
            status = "SERVING",
            index = new
            {
                ip_ranges = ipIndex.RangeCount,
                distinct_info_count = ipIndex.InfoCount
            },
            import_task = new
            {
                start_time = importTask.StartTime,
                duration = importTask.Duration
            }
        });
    }
}