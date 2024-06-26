namespace IpLookup.Api.Downloader;

/// <summary>
/// The download response contains the URI to the downloaded file.
/// </summary>
/// <param name="FileUri">The URI to the downloaded file.</param>
public readonly record struct DownloadResponse(Uri FileUri);