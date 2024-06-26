using System.IO.Compression;

namespace IpLookup.Api.Downloader;

/// <summary>
/// The default implementation of the <see cref="IDownloader"/> interface.
/// </summary>
public class Downloader() : IDownloader
{
    private readonly ILogger<Downloader> _logger;
    private readonly HttpClient _httpClient = new();

    /// <summary>
    /// Creates a new <see cref="Downloader"/> instance.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public Downloader(ILogger<Downloader> logger) : this()
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a new <see cref="Downloader"/> instance with a custom HTTP client. 
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    public Downloader(ILogger<Downloader> logger,
                      IHttpClientFactory httpClientFactory) : this(logger)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    /// <inheritdoc />
    public async Task<DownloadResponse> Download(Uri source,
                                                 string destination,
                                                 bool decompress)
    {
        var downloadPath = await DownloadFile(source, destination);
        if (decompress)
            await DecompressGZipFile(downloadPath, destination);

        _logger.LogInformation("Download completed successfully.");

        var filePath = decompress
            ? GetExtractPath(downloadPath, destination)
            : downloadPath;
        var fileUri = new Uri(filePath);

        return new DownloadResponse(fileUri);
    }

    private async Task<string> DownloadFile(Uri uri, string destination)
    {
        var fileName = Path.GetFileName(uri.AbsolutePath);
        var downloadPath = Path.Combine(destination, fileName);

        await using var dataStream = await _httpClient.GetStreamAsync(uri);
        await using var fileStream = File.Create(downloadPath);
        await dataStream.CopyToAsync(fileStream);

        return downloadPath;
    }

    private static async Task DecompressGZipFile(string filePath, string extractFolder)
    {
        var extractPath = GetExtractPath(filePath, extractFolder);

        await using var sourceStream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read
        );
        await using var destinationStream = File.Create(extractPath);
        await using var decompressionStream = new GZipStream(
            sourceStream,
            CompressionMode.Decompress
        );

        await decompressionStream.CopyToAsync(destinationStream);

        // Delete the compressed file after decompression.
        File.Delete(filePath);
    }

    private static string GetExtractPath(string filePath, string extractFolder)
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var extractPath = Path.Combine(extractFolder, fileName);
        return extractPath;
    }
}