using System.Net;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.dotTrace;
using IpLookup.Api.Downloader;
using IpLookup.Api.Import;
using IpLookup.Api.Lookup;
using IpLookup.Api.Storage.InMemory;
using Microsoft.Extensions.Logging.Abstractions;

namespace IpLookup.Benchmarks;

[DotTraceDiagnoser]
[SimpleJob]
[MarkdownExporter]
public class IpIndexBenchmark
{
    private static readonly IpIndex _ipIndex = new();

    private static readonly IpLookupService _ipLookupService =
        new(NullLogger<IpLookupService>.Instance, _ipIndex);

    [GlobalSetup]
    public void Setup()
    {
        // The required CSV file can be found at:
        // - https://github.com/sapics/ip-location-db

        var fileUri = new Uri("/srv/data/ip-datasets/dbip-city-ipv4.csv");
        var logger = NullLogger<ImportService>.Instance;
        var downloader = new Downloader();
        var importService = new ImportService(logger, downloader);

        importService.Import(fileUri, _ipIndex);
    }

    private static readonly IPAddress TestIp = IPAddress.Parse("8.8.8.8");

    [Benchmark]
    public void Get()
    {
        _ipIndex.TryGetValue(TestIp, out _);
    }

    [Benchmark]
    public void ServiceGet()
    {
        _ipLookupService.TryGetValue(TestIp, out _);
    }
}