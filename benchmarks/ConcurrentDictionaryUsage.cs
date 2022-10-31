using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

public static class ConcurrentDictionaryUsage 
{
    public static void Use()
    {
        var someState = new object();
        var someOtherState = 42;

        var dictionary = new ConcurrentDictionary<string, string>();

        dictionary.GetOrAdd("SomeKey", static (key, state) => {
            var (someState, someOtherState) = state;
            return $"{someState}_{someOtherState}";
        }, (someState1: someState, someOtherState));
    }
}

[Config(typeof(Config))]
public class ConcurrentDictionaryClosure
{
    private ConcurrentDictionary<string,string> dictionary;
    private object someState;
    private int someOtherState;

    private class Config : ManualConfig
    {
        public Config()
        {
            AddExporter(MarkdownExporter.GitHub);
            AddDiagnoser(MemoryDiagnoser.Default);
            AddJob(Job.Default.WithUnrollFactor(80000));
        }
    }

    [IterationSetup]
    public void Setup()
    {
        dictionary = new ConcurrentDictionary<string, string>();
        
        someState = new object();
        someOtherState = Random.Shared.Next(10, 99);
    }

    [Benchmark(Baseline = true)]
    public string GetOrAddWithClosure()
    {
        return dictionary.GetOrAdd("SomeKey", (key) => $"{someState}_{someOtherState}");
    }

    [Benchmark]
    public string GetOrAddWithoutClosure()
    {
        return dictionary.GetOrAdd("SomeKey", static (key, state) => {
            var (someState, someOtherState) = state;
            return $"{someState}_{someOtherState}";
        }, (someState1: someState, someOtherState));
    }
}