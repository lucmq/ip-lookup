namespace IpLookup.Api.Import;

/// <summary>
/// The options available to configure an import task.
/// </summary>
public class ImportTaskOptions
{
    /// <summary>
    /// Keep it for the framework.
    /// </summary>
    public const string ImportTask = "ImportTask";

    /// <summary>
    /// The URI of the file to import. This can be a local file or a remote
    /// address.
    /// </summary>
    public string FileUri { get; init; } = "";
}