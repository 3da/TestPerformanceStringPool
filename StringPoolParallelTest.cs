using BenchmarkDotNet.Attributes;
using CommunityToolkit.HighPerformance.Buffers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestPerformanceStringPool
{

    [SimpleJob]
    [RPlotExporter]
    [MemoryDiagnoser]
    public class StringPoolParallelTest
    {
        private List<string> _list = new List<string>();

        [Params(10, 500)]
        public int StringLength;

        [Params(2, 4, 8, 16)]
        public int Parallelism;

        [GlobalSetup]
        public void Setup()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[StringLength];
            var random = new Random(42);

            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < StringLength; j++)
                {
                    stringChars[j] = chars[random.Next(chars.Length)];
                }
                _list.Add(new string(stringChars));
            }
        }

        [Benchmark]
        public ConcurrentBag<string> StringPool()
        {
            var pool = new StringPool();
            var result = new ConcurrentBag<string>();
            Parallel.For(0, 100, new ParallelOptions
            {
                MaxDegreeOfParallelism = Parallelism
            }, i =>
            {
                foreach (var s in _list)
                {
                    result.Add(pool.GetOrAdd(s));
                }
            });
            return result;
        }

        [Benchmark]
        public ConcurrentBag<string> Intern()
        {
            var result = new ConcurrentBag<string>();
            Parallel.For(0, 100, new ParallelOptions
            {
                MaxDegreeOfParallelism = Parallelism
            }, i =>
            {
                foreach (var s in _list)
                {
                    result.Add(string.Intern(s));
                }
            });
            return result;
        }

        [Benchmark]
        public ConcurrentBag<string> ConcurrentDictionary()
        {
            var pool = new ConcurrentDictionary<string, string>();
            var result = new ConcurrentBag<string>();
            Parallel.For(0, 100, new ParallelOptions
            {
                MaxDegreeOfParallelism = Parallelism
            }, i =>
            {
                foreach (var s in _list)
                {
                    result.Add(pool.GetOrAdd(s, s));
                }
            });
            return result;
        }
    }
}
