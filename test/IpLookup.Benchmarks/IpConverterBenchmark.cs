using System.Net;
using BenchmarkDotNet.Attributes;
using IpLookup.Api.Utilities;

namespace IpLookup.Benchmarks;

public class IpConverterBenchmark
{
    private static readonly IPAddress TestIp = IPAddress.Parse("8.8.8.8");

    [Benchmark]
    public void IpConverter_IpAddressToUInt64()
    {
        IpConverter.IpAddressToUInt64(TestIp);
    }

    [Benchmark(Baseline = true)]
    public void IpAddressToUInt32()
    {
        IpAddressToUInt32(TestIp);
    }

    private static uint IpAddressToUInt32(IPAddress ipAddress)
    {
        var bytes = ipAddress.GetAddressBytes();
        var intHostOrder = (int)BitConverter.ToUInt32(bytes, 0);
        return (uint)IPAddress.NetworkToHostOrder(intHostOrder);
    }
}