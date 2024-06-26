using System.Diagnostics;
using IpLookup.Api.Storage.InMemory;
using Microsoft.Extensions.Options;

namespace IpLookup.Api.Import;

/// <summary>
/// A background task that imports an IP address index from a file.
/// </summary>
/// <param name="logger">The service logger.</param>
/// <param name="importService">The underlying import service.</param>
/// <param name="ipIndex">
/// The IP index. The IP ranges from the file will be added to this index.
/// </param>
/// <param name="options">Additional options to configure the task.</param>
public class ImportTask(
    ILogger<ImportTask> logger,
    ImportService importService,
    IIpIndex ipIndex,
    IOptions<ImportTaskOptions> options
) : IHostedService
{
    /// <summary>
    /// The start time of the import task.
    /// </summary>
    public DateTime StartTime { get; } = DateTime.UtcNow;

    /// <summary>
    /// Represents how long the import task took to complete.
    /// </summary>
    public TimeSpan Duration { get; private set; }

    /// <summary>
    /// Starts the import task.
    /// </summary>
    /// <param name="cancellationToken">Ignored. Exists for compatibility.</param>
    /// <returns></returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Runs when the application starts, and blocks the startup process until
        // finished.
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        logger.LogInformation(
            "Import Task started at {StartTime}, URI: {FileUri}",
            StartTime,
            options.Value.FileUri
        );

        var fileUri = new Uri(options.Value.FileUri);
        importService.Import(fileUri, ipIndex);

        Duration = stopwatch.Elapsed;

        logger.LogInformation(
            "Import Task finished. IP ranges: {RangeCount}, " +
            "distinct info: {InfoCount}, time elapsed: {Duration}",
            ipIndex.RangeCount,
            ipIndex.InfoCount,
            Duration
        );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the import task.
    /// </summary>
    /// <param name="cancellationToken">Ignored. Exists for compatibility.</param>
    /// <returns></returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}