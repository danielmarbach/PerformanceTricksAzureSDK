using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

[Config(typeof(Config))]
public class BodyCopying
{
    private byte[] originalBody;

    private class Config : ManualConfig
    {
        public Config()
        {
            AddExporter(MarkdownExporter.GitHub);
            AddDiagnoser(MemoryDiagnoser.Default);
            AddJob(Job.Default.WithUnrollFactor(256));
        }
    }

    [Params(16, 32, 64, 256,  1024)]
    public int Size { get; set; }

    [IterationSetup]
    public void Setup()
    {
        originalBody = Enumerable.Repeat((byte)12, Size).ToArray();
    }

    [Benchmark(Baseline = true)]
    public byte[] ArrayCopy()
    {
        var clonedBody = new byte[originalBody.Length];
        Array.Copy(originalBody.ToArray(), clonedBody, originalBody.Length);
        return clonedBody;
    }

    [Benchmark]
    public byte[] NoCopy()
    {
        return originalBody;
    }
}