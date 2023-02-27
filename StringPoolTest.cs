using BenchmarkDotNet.Attributes;
using CommunityToolkit.HighPerformance.Buffers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TestPerformanceStringPool
{

    [SimpleJob]
    [RPlotExporter]
    [MemoryDiagnoser]
    public class StringPoolTest
    {
        private List<string> _list = new List<string>();

        [Params(10, 100, 1000)]
        public int StringLength;

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
        public IList<string> StringPool()
        {
            var pool = new StringPool();
            var result = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                foreach (var s in _list)
                {
                    result.Add(pool.GetOrAdd(s));
                }
            }
            return result;
        }

        [Benchmark]
        public IList<string> Intern()
        {
            var result = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                foreach (var s in _list)
                {
                    result.Add(string.Intern(s));
                }
            }
            return result;
        }

        [Benchmark]
        public IList<string> ConcurrentDictionary()
        {
            var pool = new ConcurrentDictionary<string, string>();
            var result = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                foreach (var s in _list)
                {
                    result.Add(pool.GetOrAdd(s, s));
                }
            }
            return result;
        }

        [Benchmark]
        public IList<string> Dictionary()
        {
            var pool = new Dictionary<string, string>();
            var result = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                foreach (var s in _list)
                {
                    if (!pool.TryGetValue(s, out var t))
                    {
                        t = s;
                        pool.Add(s, s);
                    }
                    result.Add(t);
                }
            }
            return result;
        }
    }
}
