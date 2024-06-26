using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using IpLookup.Api.Downloader;
using IpLookup.Api.Lookup;
using IpLookup.Api.Storage.InMemory;

namespace IpLookup.Api.Import;

/// <summary>
/// The service used to import an IP address index from a file.
/// </summary>
public class ImportService
{
    /// <summary>
    /// The default download destination, when importing from a remote file. The
    /// remote file will first be decompressed into this location before being
    /// imported.
    /// </summary>
    public string DefaultDownloadDestination = Path.GetTempPath();

    private readonly ILogger<ImportService> _logger;
    private readonly IDownloader _downloader;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportService"/> class.
    /// </summary>
    /// <param name="logger">The logger used for logging.</param>
    /// <param name="downloader">The downloader used to download files.</param>
    public ImportService(ILogger<ImportService> logger, IDownloader downloader)
    {
        _logger = logger;
        _downloader = downloader;
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportService"/> class using
    /// a custom <see cref="IHttpClientFactory"/>.
    /// </summary>
    /// <param name="logger">The logger used for logging.</param>
    /// <param name="downloader">The downloader used to download files.</param>
    public ImportService(ILogger<ImportService> logger,
                         IDownloader downloader,
                         IHttpClientFactory httpClientFactory) : this(logger, downloader)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    /// <summary>
    /// Imports data from a CSV file specified by a URI into an IP index.
    /// </summary>
    /// <param name="fileUri">The URI of the file to import.</param>
    /// <param name="ipIndex">The IP index to import the data into.</param>
    public void Import(Uri fileUri, IIpIndex ipIndex)
    {
        var reader = GetFileReader(fileUri);
        Import(reader, ipIndex);
    }

    /// <summary>
    /// Imports data from a CSV file stream into an IP index.
    /// </summary>
    /// <param name="reader">The stream reader used to read the CSV file.</param>
    /// <param name="index">The IP index to import the data into.</param>
    public void Import(StreamReader reader, IIpIndex index)
    {
        foreach (var row in ReadCsv(reader))
        {
            var ipInfo = row.IpInfo();
            var startIp = IPAddress.Parse(row.IpRangeStart);
            var endIp = IPAddress.Parse(row.IpRangeEnd);
            index.Add(startIp, endIp, ipInfo);
        }
    }

    /// <summary>
    /// Creates a <see cref="StreamReader"/> for the IP ranges CSV file at the
    /// specified URI.
    /// </summary>
    /// <param name="uri">The URI of the CSV file.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">
    /// The URI is not well-formed or a unsupported scheme was used.
    /// </exception>
    public StreamReader GetFileReader(Uri uri)
    {
        return uri.Scheme switch
        {
            "http" or "https" => HandleRemoteFileAsync(uri).Result,
            "file" => StreamLocalFile(uri),
            _ => throw new ArgumentException("unsupported scheme")
        };
    }

    private async Task<StreamReader> HandleRemoteFileAsync(Uri uri)
    {
        if (NeedsDownload(uri))
        {
            // Download and Decompress from remote.
            var decompress = true;
            var destination = DefaultDownloadDestination;

            _logger.LogInformation(
                "Starting download, source: {source}, destination: {dest}",
                uri,
                destination);

            var response = await _downloader.Download(uri, destination, decompress);
            return StreamLocalFile(response.FileUri);
        }

        return await StreamRemoteFileAsync(uri);
    }

    private static bool NeedsDownload(Uri uri)
    {
        var extension = Path.GetExtension(uri.AbsolutePath);
        var culture = CultureInfo.InvariantCulture;
        return extension.ToLower(culture) switch
        {
            ".gz" or ".zip" or ".7z" => true,
            _ => false
        };
    }

    private async Task<StreamReader> StreamRemoteFileAsync(Uri uri)
    {
        var stream = await _httpClient.GetStreamAsync(uri);
        return new StreamReader(stream);
    }

    private static StreamReader StreamLocalFile(Uri uri)
    {
        var filePath = uri.AbsolutePath;
        return new StreamReader(filePath);
    }

    private static IEnumerable<Row> ReadCsv(TextReader reader)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false
        };

        using var csv = new CsvReader(reader, config);
        while (csv.Read())
        {
            Debug.Assert(csv.ColumnCount == Row.ExpectedCsvColumnCount);

            var record = csv.GetRecord<Row>();
            yield return record;
        }
    }

    [SuppressMessage("ReSharper",
                     "UnusedAutoPropertyAccessor.Local",
                     Justification = "Used by CsvHelper")]
    private readonly record struct Row
    {
        public const int ExpectedCsvColumnCount = 10;

        [Index(0)] public string IpRangeStart { get; init; }
        [Index(1)] public string IpRangeEnd { get; init; }
        [Index(2)] public string Country { get; init; }
        [Index(3)] public string Region { get; init; }
        [Index(5)] public string City { get; init; }
        [Index(7)] public string Latitude { get; init; }
        [Index(8)] public string Longitude { get; init; }

        public IpInfo IpInfo()
        {
            return new IpInfo(City, Region, Country)
            {
                Latitude = Convert.ToSingle(Latitude),
                Longitude = Convert.ToSingle(Longitude)
            };
        }
    }
}