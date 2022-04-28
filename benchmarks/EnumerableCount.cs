using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

[Config(typeof(Config))]
public class EnumerableCount
{
    private List<object> data;

    private class Config : ManualConfig
    {
        public Config()
        {
            AddExporter(MarkdownExporter.GitHub);
            AddDiagnoser(MemoryDiagnoser.Default);
            AddJob(Job.ShortRun.WithInvocationCount(16000000));
        }
    }
    [IterationSetup]
    public void Setup()
    {
        data = Enumerable.Repeat(new SomeClass(), NumberOfItems).Cast<object>().ToList();
    }

    [Params(0, 1, 10, 32, 50)]
    public int NumberOfItems { get; set; }

    public class SomeClass { }

    [Benchmark(Baseline = true)]
    public List<SomeClass> Original()
    {
        var copyList = new List<SomeClass>();
        foreach (var value in GetValue<SomeClass>())
        {
            copyList.Add(value);
        }

        return copyList;
    }

    IEnumerable<TValue> GetValue<TValue>()
    {
        return data.Cast<TValue>();
    }

    [Benchmark]
    public List<SomeClass> List()
    {
        List<SomeClass> copyList = null;
        var enumerable = GetValueList<SomeClass>();
        foreach (var value in enumerable)
        {
            copyList ??= enumerable is IReadOnlyCollection<SomeClass> readOnlyList
                ? new List<SomeClass>(readOnlyList.Count)
                : new List<SomeClass>();

            copyList.Add(value);
        }

        return copyList;
    }

    IEnumerable<TValue> GetValueList<TValue>()
    {
        List<TValue> values = null;
        foreach (var item in data)
        {
            values ??= new List<TValue>(data.Count);
            values.Add((TValue)item);
        }
        return values ?? Enumerable.Empty<TValue>();
    }

    [Benchmark]
    public List<SomeClass> ListReturnList()
    {
        List<SomeClass> copyList = null;
        var enumerable = GetValueListReturnList<SomeClass>();
        foreach (var value in enumerable)
        {
            copyList ??= new List<SomeClass>(enumerable.Count);

            copyList.Add(value);
        }

        return copyList;
    }

    List<TValue> GetValueListReturnList<TValue>()
    {
        var values = new List<TValue>(data.Count);
        foreach (var item in data)
        {
            values.Add((TValue)item);
        }
        return values;
    }

    [Benchmark]
    public List<SomeClass> ListReturnListFor()
    {
        List<SomeClass> copyList = null;
        var enumerable = GetValueListReturnListFor<SomeClass>();
        foreach (var value in enumerable)
        {
            copyList ??= new List<SomeClass>(enumerable.Count);

            copyList.Add(value);
        }

        return copyList;
    }

    [Benchmark]
    public List<SomeClass> ListForReturnListFor()
    {
        List<SomeClass> copyList = null;
        var enumerable = GetValueListReturnListFor<SomeClass>();
        for (var index = 0; index < enumerable.Count; index++)
        {
            var value = enumerable[index];
            copyList ??= new List<SomeClass>(enumerable.Count);

            copyList.Add(value);
        }

        return copyList;
    }

    List<TValue> GetValueListReturnListFor<TValue>()
    {
        var values = new List<TValue>(data.Count);
        for (var index = 0; index < data.Count; index++)
        {
            var item = data[index];
            values.Add((TValue)item);
        }

        return values;
    }
}