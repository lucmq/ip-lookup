namespace IpLookup.Api.Downloader;

/// <summary>
/// The downloader interface provides methods to download files and decompress them
/// into a specified destination.
/// </summary>
public interface IDownloader
{
    /// <summary>
    /// Downloads a file from the specified source and decompresses it into the
    /// specified destination.
    /// </summary>
    /// <param name="source">The Uri to the file to download.</param>
    /// <param name="destination">The destination folder.</param>
    /// <param name="decompress">Whether to decompress the file.</param>
    /// <returns></returns>
    Task<DownloadResponse> Download(Uri source, string destination, bool decompress);
}