using System.Buffers;
using System.Buffers.Binary;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

[Config(typeof(Config))]
public class BitConvertion
{
    private byte[] originalBody;

    private class Config : ManualConfig
    {
        public Config()
        {
            AddExporter(MarkdownExporter.GitHub);
            AddDiagnoser(MemoryDiagnoser.Default);
            AddJob(Job.Default.WithUnrollFactor(1024));
        }
    }

    [IterationSetup]
    public void Setup()
    {
        originalBody = Encoding.UTF8.GetBytes("123456789101112");
    }

    [Benchmark(Baseline = true)]
    public uint BitConverterUInt32()
    {
        return BitConverter.ToUInt32(originalBody, 3);
    }

    [Benchmark]
    public uint BinaryPrimitivesRead()
    {
        return BinaryPrimitives.ReadUInt32LittleEndian(originalBody.AsSpan().Slice(3));
    }
}