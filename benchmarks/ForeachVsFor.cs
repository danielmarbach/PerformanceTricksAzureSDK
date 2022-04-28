using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

[Config(typeof(Config))]
[DisassemblyDiagnoser(printSource: true)]
public class ArrayForeachVsFor
{
    private int[] array;

    private Consumer consumer;

    private class Config : ManualConfig
    {
        public Config()
        {
            AddExporter(MarkdownExporter.GitHub);
            AddColumn(StatisticColumn.AllStatistics);
        }
    }
    [Params(0, 1, 2, 4)]
    public int Elements { get; set; }

    [GlobalSetup]
    public void SetUp()
    {
        array = Enumerable.Range(0, Elements).ToArray();

        consumer = new Consumer();
    }

    [Benchmark(Baseline = true)]
    public void Foreach()
    {
        foreach (var i in array)
        {
            consumer.Consume(i);
        }
    }

    [Benchmark]
    public void For()
    {
        for (int i = 0; i < array.Length; i++)
        {
            consumer.Consume(array[i]);
        }
    }
}

[Config(typeof(Config))]
[DisassemblyDiagnoser(printSource: true)]
public class ListForeachVsFor
{
    private List<int> list;

    private Consumer consumer;

    private class Config : ManualConfig
    {
        public Config()
        {
            AddExporter(MarkdownExporter.GitHub);
            AddColumn(StatisticColumn.AllStatistics);
        }
    }
    [Params(0, 1, 2, 4)]
    public int Elements { get; set; }

    [GlobalSetup]
    public void SetUp()
    {
        list = Enumerable.Range(0, Elements).ToList();

        consumer = new Consumer();
    }

    [Benchmark(Baseline = true)]
    public void Foreach()
    {
        foreach (var i in list)
        {
            consumer.Consume(i);
        }
    }

    [Benchmark]
    public void For()
    {
        for (int i = 0; i < list.Count; i++)
        {
            consumer.Consume(list[i]);
        }
    }
}