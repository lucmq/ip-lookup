using BenchmarkDotNet.Running;

namespace IpLookup.Benchmarks;

internal static class Program
{
    // Run with: dotnet run --configuration Release
    //
    // Running with dotTrace:
    //  - https://blog.jetbrains.com/dotnet/2023/07/11/dottrace-comes-to-benchmarkdotnet/

    private static void Main()
    {
        BenchmarkRunner.Run<IpIndexBenchmark>();
    }
}