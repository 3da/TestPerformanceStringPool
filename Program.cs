using System;

namespace TestPerformanceStringPool
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkDotNet.Running.BenchmarkRunner.Run<StringPoolTest>();
            BenchmarkDotNet.Running.BenchmarkRunner.Run<StringPoolParallelTest>();
        }
    }
}
